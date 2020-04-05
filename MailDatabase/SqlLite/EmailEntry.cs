using SQLite;

namespace MailDatabase
{
	public class EmailEntry : IEmailEntry
	{
		[PrimaryKey]
		public string EmailUser { get; set; }

		public bool Blacklisted { get; set; }
	}
}
