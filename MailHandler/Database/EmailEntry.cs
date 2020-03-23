using MailHandler.Interfaces;
using SQLite;

namespace MailHandler.Database
{
	public class EmailEntry : IEmailEntry
	{
		[PrimaryKey]
		public string EmailUser { get; set; }

		public bool Blacklisted { get; set; }
	}
}
