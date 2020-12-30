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
	/// <summary>
	/// A sender which uses MailKit as backend
	/// </summary>
	/// <seealso cref="MailHandler.Interfaces.IEmailSender" />
	public class MailKitSender : IEmailSender
	{
		private const string Localhost = "127.0.0.1";

		/// <summary>
		/// The options
		/// </summary>
		private readonly Options _options;

		/// <summary>
		/// Initializes a new instance of the <see cref="MailKitSender"/> class.
		/// </summary>
		/// <param name="options">The injected options.</param>
		public MailKitSender(Options options)
		{
			_options = options;
		}

		/// <inheritdoc/>
		public void SendEmail(Email email)
		{
			// First build the body
			BodyBuilder bodyBuilder = new BodyBuilder();
			bodyBuilder.TextBody = email.TextContents.FirstOrDefault(text => text.MimeType == Constants.TextMimeType)?.RawText;
			bodyBuilder.HtmlBody = email.TextContents.FirstOrDefault(text => text.MimeType == Constants.HtmlMimeType)?.RawText;

			foreach (Interfaces.Models.Attachment attachment in email.Attachments)
			{
				bodyBuilder.Attachments.Add(GetAttachment(attachment));
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

		/// <summary>
		/// Gets the internet address as an MimeKit object.
		/// </summary>
		/// <param name="mailAddress">The mail address.</param>
		/// <returns>
		/// The email address as an MimeKit object.
		/// </returns>
		private static InternetAddress GetInternetAddress(MailAddress mailAddress)
		{
			return InternetAddress.Parse(mailAddress.ToString());
		}

		/// <summary>
		/// Gets the attachment as an MimeKit object.
		/// </summary>
		/// <param name="attachment">The attachment.</param>
		/// <returns>
		/// The attachment as an MimeKit object
		/// </returns>
		private static MimeEntity GetAttachment(Interfaces.Models.Attachment attachment)
		{
			return new MimePart(ContentType.Parse(attachment.ContentType))
			{
				FileName = attachment.DisplayFileName,
				Content = new MimeContent(File.OpenRead(attachment.FilePath)),
				ContentTransferEncoding = attachment.Encoding.GetContentEncoding(),
				IsAttachment = true,
			};
		}

	}
}
