﻿using MailDatabase;
using MailHandler.Cache;
using MailHandler.Forwarding.Meta;
using MailHandler.Interfaces;
using MailHandler.Interfaces.Models;
using MimeKit;
using System.IO;

namespace MailHandler.Forwarding
{
	/// <summary>
	/// An email handler which forwards messages towards another email address for storage purposes. This forward handler is
	/// forward only currently, not yet reversible.
	/// </summary>
	/// <seealso cref="MailHandler.Interfaces.IEmailHandler" />
	public class Forwarder : IEmailHandler
	{
		private const string OriginalToHeader = "X-Original-To";
		private const string LocalSuffix = ".local";

		private readonly Options _options;
		private readonly IEmailSender _sender;

		private readonly SessionObjectCache<string, IEmailEntry> emailEntryCache;
		private readonly SessionObjectCache<MimeMessage, string> toCache;

		/// <summary>
		/// Initializes a new instance of the <see cref="Forwarder"/> class.
		/// </summary>
		/// <param name="options">The injected options.</param>
		/// <param name="emailSender">The injected email sender.</param>
		/// <param name="database">The injected database.</param>
		public Forwarder(Options options, IEmailSender emailSender, IEmailDatabase database)
		{
			_options = options;
			_sender = emailSender;

			emailEntryCache = new SessionObjectCache<string, IEmailEntry>(database.Find);
			toCache = new SessionObjectCache<MimeMessage, string>((message) =>
			{
				string to = message.Headers[OriginalToHeader];
				// Check if the to is a local user
				if (to.Contains(Constants.AtSymbol))
				{
					System.Net.Mail.MailAddress address = new System.Net.Mail.MailAddress(to);
					return address.User;
				}
				else
				{
					// probably a local user
					return to + LocalSuffix;
				}
			});
		}

		/// <inheritdoc/>
		public bool HandleIncomingEmail()
		{
			MimeParser mimeParser = new MimeParser(_options.GetInputEmail(), !_options.StdIn);
			MimeMessage message = mimeParser.ParseMessage();
			if (!message.Headers.Contains(OriginalToHeader))
			{
				return false;
			}

			string from = message.From[0].ToString();
			string to = toCache.Get(message);

			if (IsFromRelay(from))
			{
				// Not supported yet
				return false;
			}
			else if (ShouldForward(to))
			{
				ForwardEmail(message);
			}

			return true;
		}

		/// <summary>
		/// Forwards the email to the relay email address.
		/// </summary>
		/// <param name="mimeMessage">The MIME message.</param>
		public void ForwardEmail(MimeMessage mimeMessage)
		{
			Metadata metadata = MetadataFactory.GenerateFrom(mimeMessage);
			// Add tags to the subject
			string subject = mimeMessage.Subject;
			IEmailEntry emailEntry = emailEntryCache.Get(toCache.Get(mimeMessage));
			if (!string.IsNullOrEmpty(emailEntry?.Tag))
			{
				metadata.Tag = emailEntry.Tag;
				subject = $"[{emailEntry.Tag}] {subject}";
			}

			Email email = new Email(
				new System.Net.Mail.MailAddress(_options.GetSender()),
				new System.Net.Mail.MailAddress(_options.RelayEmail))
			{
				Subject = subject,
			};

			foreach (MimeEntity mimeEntity in mimeMessage.BodyParts)
			{
				if (mimeEntity.IsAttachment)
				{
					if (!(mimeEntity is MimePart mimePart)) continue;

					Attachment temporaryAttachment;
					using (MemoryStream memoryStream = new MemoryStream())
					{
						mimePart.Content.DecodeTo(memoryStream);
						memoryStream.Seek(0, SeekOrigin.Begin);

						temporaryAttachment = new TemporaryAttachment(memoryStream)
						{
							DisplayFileName = mimePart.FileName,
							ContentType = mimePart.ContentType.MimeType,
							Encoding = mimePart.ContentTransferEncoding.GetRaw(),
						};
					}

					email.Attachments.Add(temporaryAttachment);
				}
				else if (mimeEntity is TextPart textPart)
				{
					string text = textPart.Text;
					if (textPart.ContentType.MimeType == Constants.TextMimeType)
					{
						text = MetadataSerializer.SerializeForText(metadata, text);
					}
					else if (textPart.ContentType.MimeType == Constants.HtmlMimeType)
					{
						text = MetadataSerializer.SerializeForHtml(metadata, text);
					}

					email.TextContents.Add(
						new Text()
						{
							RawText = text,
							MimeType = textPart.ContentType.MimeType,
							Charset = textPart.ContentType.Charset,
						});
				}
			}

			_sender.SendEmail(email);

			emailEntryCache.Clear();
			toCache.Clear();
		}

		/// <summary>
		/// Checks whether we should forward the receiving email.
		/// </summary>
		/// <param name="email">The email.</param>
		/// <returns>
		/// Whether the email should be forwarded to the relay email
		/// </returns>
		public bool ShouldForward(string email)
		{
			IEmailEntry emailEntry = emailEntryCache.Get(email);
			if (emailEntry != null)
			{
				return !emailEntry.Blacklisted;
			}
			return true;
		}

		/// <summary>
		/// Determines whether the specified email is the relay email.
		/// </summary>
		/// <param name="email">The email.</param>
		/// <returns>
		///   <c>true</c> if the specified email is the relay email; otherwise, <c>false</c>.
		/// </returns>
		public bool IsFromRelay(string email)
		{
			return string.Equals(_options.RelayEmail, email, System.StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
