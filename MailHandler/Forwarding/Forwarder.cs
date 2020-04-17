using MailDatabase;
using MailHandler.Cache;
using MailHandler.Forwarding.Meta;
using MailHandler.Interfaces;
using MailHandler.Interfaces.Models;
using MimeKit;
using MimeKit.Text;
using System.IO;

namespace MailHandler.Forwarding
{
	public class Forwarder : IEmailHandler
	{
		private const string OriginalToHeader = "X-Original-To";
		private const string LocalSuffix = ".local";

		private readonly Options _options;
		private readonly IEmailSender _sender;

		private readonly SessionObjectCache<string, IEmailEntry> emailEntryCache;
		private readonly SessionObjectCache<MimeMessage, string> toCache;

		public Forwarder(Options options, IEmailSender emailSender, IEmailDatabase database)
		{
			_options = options;
			_sender = emailSender;

			emailEntryCache = new SessionObjectCache<string, IEmailEntry>(database.Find);
			toCache = new SessionObjectCache<MimeMessage, string>((message) =>
			{
				string to = message.Headers[OriginalToHeader];
				// Check if the to is a local user
				if (to.Contains("@"))
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
				TextContent = MetadataSerializer.SerializeWithText(metadata, mimeMessage.GetTextBody(TextFormat.Plain)),
				HtmlContent = MetadataSerializer.SerializeWithHtml(metadata, mimeMessage.GetTextBody(TextFormat.Html)),
				Subject = subject,
			};

			foreach (MimeEntity mimeEntity in mimeMessage.BodyParts)
			{
				if (mimeEntity.IsAttachment)
				{
					MimePart mimePart = mimeEntity as MimePart;
					if (mimePart == null) continue;

					TemporaryAttachment temporaryAttachment;
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
			}

			_sender.SendEmail(email);

			emailEntryCache.Clear();
			toCache.Clear();
		}

		public bool ShouldForward(string email)
		{
			IEmailEntry emailEntry = emailEntryCache.Get(email);
			if (emailEntry != null)
			{
				return !emailEntry.Blacklisted;
			}
			return true;
		}

		public bool IsFromRelay(string email)
		{
			return string.Equals(_options.RelayEmail, email, System.StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
