using SQLite;

namespace MailDatabase.SqlLite
{
	public class EmailDatabase : IEmailDatabase
	{
		private readonly SQLiteConnection _connection;

		public EmailDatabase(string path)
		{
			if (!string.IsNullOrEmpty(path))
			{
				_connection = new SQLiteConnection(path);
				CreateTables();
			}
		}

		public EmailDatabase(SQLiteConnection connection)
		{
			if (connection != null)
			{
				_connection = connection;
				CreateTables();
			}
		}

		private void CreateTables()
		{
			_connection.CreateTable<EmailEntry>();
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
