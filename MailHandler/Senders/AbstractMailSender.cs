using MailHandler.Interfaces;
using System.Collections.Generic;

namespace MailHandler.Senders
{
	public abstract class AbstractMailSender : IEmailSender
	{
		public void SendEmail(string from, string to, string subject, string textBody, string htmlContent)
		{
			SendEmail(from, new string[] { to }, subject, textBody, htmlContent);
		}

		public abstract void SendEmail(string from, IEnumerable<string> to, string subject, string textBody, string htmlContent);
	}
}
