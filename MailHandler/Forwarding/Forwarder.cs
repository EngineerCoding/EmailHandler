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
		private readonly Options _options;
		private readonly IEmailSender _sender;

		private readonly SessionObjectCache<string, IEmailEntry> emailEntryCache;

		public Forwarder(Options options, IEmailSender emailSender, IEmailDatabase database)
		{
			_options = options;
			_sender = emailSender;

			emailEntryCache = new SessionObjectCache<string, IEmailEntry>(database.FindEmailEntry);
		}

		public bool HandleIncomingEmail()
		{
			MimeParser mimeParser = new MimeParser(_options.GetInputEmail(), !_options.StdIn);
			MimeMessage message = mimeParser.ParseMessage();
			string from = message.From[0].ToString();

			if (IsFromRelay(from))
			{
				// Not supported yet
				return false;
			}
			else if (ShouldForward(from))
			{
				ForwardEmail(message);
			}
			
			return true;
		}

		public void ForwardEmail(MimeMessage mimeMessage)
		{
			Metadata metadata = MetadataFactory.GenerateFrom(mimeMessage);
			Email email = new Email(
				new System.Net.Mail.MailAddress(_options.GetSender()),
				new System.Net.Mail.MailAddress(_options.RelayEmail))
			{
				TextContent = MetadataSerializer.SerializeWithText(metadata, mimeMessage.GetTextBody(TextFormat.Plain)),
				HtmlContent = MetadataSerializer.SerializeWithHtml(metadata, mimeMessage.GetTextBody(TextFormat.Html)),
			};

			// Add tags to the subject
			email.Subject = mimeMessage.Subject;
			IEmailEntry emailEntry = emailEntryCache.Get(metadata.From);
			emailEntryCache.Clear();
			if (!string.IsNullOrEmpty(emailEntry?.Tag))
			{
				email.Subject = $"[{emailEntry.Tag}] {email.Subject}";
			}			
	
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
