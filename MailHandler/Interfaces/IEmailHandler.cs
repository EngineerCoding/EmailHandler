namespace MailHandler.Interfaces
{
	/// <summary>
	/// An abstract email handler.
	/// </summary>
	public interface IEmailHandler
	{
		/// <summary>
		/// Handles the incoming email.
		/// </summary>
		/// <returns>
		/// Whether the mail was handled correctly.
		/// </returns>
		bool HandleIncomingEmail();
	}
}
