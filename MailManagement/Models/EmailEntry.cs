using MailDatabase;

namespace MailManagement.Models
{
	public class EmailEntry : IEmailEntry
	{
		public string EmailUser { get; set; }
		public bool Blacklisted { get; set; }
		public string Tag { get; set; }
	}
}
