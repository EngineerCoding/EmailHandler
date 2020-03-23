using MailHandler.Interfaces.Models;

namespace MailHandler.Interfaces
{
	public interface IEmailSender
	{
		void SendEmail(Email email);
	}
}
