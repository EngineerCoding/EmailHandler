using MailHandler.Senders.GnuMailCommand;
using System.Collections.Generic;

namespace MailHandler.Senders
{
	public class GnuMailSender : AbstractMailSender
	{
		public override void SendEmail(string from, IEnumerable<string> to, string subject, string textBody, string htmlContent)
		{
			CommandBuilder builder = new CommandBuilder()
				.SetFrom(from)
				.SetSubject(subject);

			if (!string.IsNullOrEmpty(textBody))
			{
				builder.AttachContent(ContentType.Plain, textBody);
			}
			
			if (!string.IsNullOrEmpty(htmlContent))
			{
				builder.AttachContent(ContentType.Html, htmlContent);
			}

			builder.Build(to).Execute();
		}
	}
}
