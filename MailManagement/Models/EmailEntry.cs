using MailDatabase;

namespace MailManagement.Models
{
	public class EmailEntry : IEmailEntry
	{
		public virtual string EmailUser { get; set; }
		public virtual bool Blacklisted { get; set; }
		public virtual string Tag { get; set; }
	}
}
