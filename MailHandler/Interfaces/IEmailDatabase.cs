namespace MailHandler.Interfaces
{
	public interface IEmailDatabase
	{
		public IEmailEntry FindEmailEntry(string emailUser);
	}
}
