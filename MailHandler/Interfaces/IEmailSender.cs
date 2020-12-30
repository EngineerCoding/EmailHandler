using MailHandler.Interfaces.Models;

namespace MailHandler.Interfaces
{
	/// <summary>
	/// An email backend which sends emails
	/// </summary>
	public interface IEmailSender
	{
		/// <summary>
		/// Sends the email.
		/// </summary>
		/// <param name="email">The email.</param>
		void SendEmail(Email email);
	}
}
