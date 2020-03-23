using MailHandler.Interfaces;
using MailHandler.Interfaces.Models;
using MailHandler.Senders.GnuMailCommand;

namespace MailHandler.Senders
{
	public class GnuMailSender : IEmailSender
	{
		public void SendEmail(Email email)
		{
			CommandBuilder builder = new CommandBuilder
			{
				Subject = email.Subject,
				ReturnAddress = email.From,
				TextContent = email.GetTextContentAsAttachment(),
				HtmlContent = email.GetHtmlContentAsAttachment(),
			};
			builder.AddAttachments(email.Attachments);

			builder.Build(email.To).Execute();
		}
	}
}
