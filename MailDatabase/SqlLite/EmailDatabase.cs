using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace MailDatabase.SqlLite
{
	/// <summary>
	/// Implementations for an SQLdatabase
	/// </summary>
	public class EmailDatabase : IEmailDatabase
	{
		/// <summary>
		/// The connection
		/// </summary>
		private readonly SQLiteConnection _connection;

		/// <summary>
		/// Initializes a new instance of the <see cref="EmailDatabase"/> class.
		/// </summary>
		/// <param name="path">The path to the SQlite database (or to create at thois path).</param>
		public EmailDatabase(string path)
		{
			if (!string.IsNullOrEmpty(path))
			{
				_connection = new SQLiteConnection(path);
				CreateTables();
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EmailDatabase"/> class.
		/// </summary>
		/// <param name="connection">The connection.</param>
		public EmailDatabase(SQLiteConnection connection)
		{
			if (connection != null)
			{
				_connection = connection;
				CreateTables();
			}
		}

		/// <summary>
		/// Creates the tables.
		/// </summary>
		private void CreateTables()
		{
			_connection.CreateTable<EmailEntry>();
		}

		/// <inheritdoc/>
		public IEmailEntry Find(string emailUser)
		{
			if (_connection == null)
			{
				return null;
			}
			return _connection.Find<EmailEntry>(emailUser);
		}

		/// <inheritdoc/>
		public void Insert(IEmailEntry emailEntry)
		{
			_connection?.Insert(emailEntry);
		}

		/// <inheritdoc/>
		public void Update(IEmailEntry emailEntry)
		{
			_connection?.Update(emailEntry);
		}

		/// <inheritdoc/>
		public void Delete(IEmailEntry emailEntry)
		{
			_connection?.Delete(emailEntry);
		}

		/// <inheritdoc/>
		public IEnumerable<IEmailEntry> All(int? skip = null, int? take = null)
		{
			if (_connection == null)
			{
				return Enumerable.Empty<IEmailEntry>().AsQueryable();
			}

			TableQuery<EmailEntry> query = _connection.Table<EmailEntry>();
			if (skip.HasValue)
			{
				query = query.Skip(skip.Value);
			}
			if (take.HasValue)
			{
				query = query.Take(take.Value);
			}

			return query;
		}

		/// <inheritdoc/>
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
