using MailHandler.Interfaces;
using SQLite;

namespace MailHandler.Database
{
	public class EmailDatabase : IEmailDatabase
	{
		private readonly SQLiteConnection _connection;

		public EmailDatabase(Options options)
		{
			if (string.IsNullOrEmpty(options.EmailDatabase))
			{
				_connection = null;
			}
			else
			{
				_connection = new SQLiteConnection(options.EmailDatabase);
				_connection.CreateTable<EmailEntry>();
			}
		}

		public IEmailEntry FindEmailEntry(string emailUser)
		{
			if (_connection == null)
			{
				return null;
			}
			return _connection.Find<EmailEntry>(emailUser);
		}
	}
}
