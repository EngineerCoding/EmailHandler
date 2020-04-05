namespace MailDatabase
{
	public interface IEmailDatabase
	{
		IEmailEntry FindEmailEntry(string emailUser);
	}
}
