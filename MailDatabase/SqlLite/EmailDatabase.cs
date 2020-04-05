using SQLite;
using System;
using System.Collections.Generic;

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

		public IEmailEntry Find(string emailUser)
		{
			if (_connection == null)
			{
				return null;
			}
			return _connection.Find<EmailEntry>(emailUser);
		}

		public void Insert(IEmailEntry emailEntry)
		{
			_connection?.Insert(emailEntry);
		}

		public void Update(IEmailEntry emailEntry)
		{
			_connection?.Update(emailEntry);
		}

		public void Delete(IEmailEntry emailEntry)
		{
			_connection?.Delete(emailEntry);
		}

		public IEnumerable<IEmailEntry> All()
		{
			if (_connection == null)
			{
				return Array.Empty<IEmailEntry>();
			}
			return _connection.Table<EmailEntry>();
		}

		public int GetCount()
		{
			if (_connection == null)
			{
				return 0;
			}
			return _connection.Table<EmailEntry>().Count();
		}
	}
}
