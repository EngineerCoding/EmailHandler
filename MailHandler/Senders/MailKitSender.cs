using MailHandler.Forwarding;
using MailHandler.Interfaces;
using MailHandler.Interfaces.Models;
using MimeKit;
using System.IO;
using System.Linq;
using System.Net.Mail;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace MailHandler.Senders
{
	public class MailKitSender : IEmailSender
	{
		private const string Localhost = "127.0.0.1";

		private readonly Options _options;

		public MailKitSender(Options options)
		{
			_options = options;
		}

		public void SendEmail(Email email)
		{
			// First build the body
			BodyBuilder bodyBuilder = new BodyBuilder();
			bodyBuilder.TextBody = email.Texts.FirstOrDefault(text => text.MimeType == Constants.TextMimeType)?.RawText;
			bodyBuilder.HtmlBody = email.Texts.FirstOrDefault(text => text.MimeType == Constants.HtmlMimeType)?.RawText;

			foreach (Interfaces.Models.Attachment attachment in email.Attachments)
			{
				MimePart mimePart = new MimePart(ContentType.Parse(attachment.ContentType))
				{
					FileName = attachment.DisplayFileName,
					Content = new MimeContent(File.OpenRead(attachment.FilePath)),
					ContentTransferEncoding = attachment.Encoding.GetContentEncoding().Value,
					IsAttachment = true,
				};

				bodyBuilder.Attachments.Add(mimePart);
			}

			// Build the message
			MimeMessage mimeMessage = new MimeMessage();
			mimeMessage.Subject = email.Subject;
			mimeMessage.Body = bodyBuilder.ToMessageBody();
			mimeMessage.From.Add(GetInternetAddress(email.From));

			foreach (MailAddress from in email.To)
			{
				mimeMessage.To.Add(GetInternetAddress(from));
			}

			// Send the message
			using (SmtpClient smtpClient = new SmtpClient())
			{
				smtpClient.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
				smtpClient.Connect(Localhost, _options.SmtpPort);
				smtpClient.Send(mimeMessage);
			}
		}

		private static InternetAddress GetInternetAddress(MailAddress mailAddress)
		{
			return InternetAddress.Parse(mailAddress.ToString());
		}
	}
}
