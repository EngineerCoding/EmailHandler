using System.Collections.Generic;

namespace MailHandler.Interfaces
{
	public interface IEmailSender
	{
		void SendEmail(string from, string to, string subject, string textBody, string htmlContent);

		void SendEmail(string from, IEnumerable<string> to, string subject, string textBody, string htmlContent);
	}
}
