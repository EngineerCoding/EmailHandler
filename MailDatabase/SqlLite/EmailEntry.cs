using SQLite;

namespace MailDatabase
{
	/// <summary>
	/// The database model as an email entry
	/// </summary>
	/// <seealso cref="MailDatabase.IEmailEntry" />
	public class EmailEntry : IEmailEntry
	{
		/// <inheritdoc/>
		[PrimaryKey]
		public string EmailUser { get; set; }

		/// <inheritdoc/>
		public bool Blacklisted { get; set; }

		/// <inheritdoc/>
		public string Tag { get; set; }
	}
}
