using MailDatabase;

namespace MailManagement.Models
{
	public class ProxyEmailEntry : EmailEntry
	{
		private readonly IEmailEntry _backingEntry;

		public ProxyEmailEntry(IEmailEntry backingEntry)
		{
			_backingEntry = backingEntry;
		}

		public override string EmailUser
		{
			get => _backingEntry.EmailUser;
			set => _backingEntry.EmailUser = value;
		}

		public override bool Blacklisted
		{
			get => _backingEntry.Blacklisted;
			set => _backingEntry.Blacklisted = value;
		}

		public override string Tag
		{
			get => _backingEntry.Tag;
			set => _backingEntry.Tag = value;
		}
	}
}
