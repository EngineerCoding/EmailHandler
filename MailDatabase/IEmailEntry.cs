namespace MailDatabase
{
	public interface IEmailEntry
	{
		string EmailUser { get; set; }

		bool Blacklisted { get; set; }
	}
}
