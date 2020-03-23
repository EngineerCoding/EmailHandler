namespace MailHandler.Interfaces
{
	public interface IEmailEntry
	{
		string EmailUser { get; }

		bool Blacklisted { get; }
	}
}
