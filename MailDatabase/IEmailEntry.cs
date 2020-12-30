namespace MailDatabase
{
	/// <summary>
	/// Represents an entry in the email database
	/// </summary>
	public interface IEmailEntry
	{
		/// <summary>
		/// The user on this domain (before the '@' part of your mail)
		/// </summary>
		string EmailUser { get; set; }

		/// <summary>
		/// Whether this particular email is blacklisted, and therefore not processed
		/// </summary>
		bool Blacklisted { get; set; }

		/// <summary>
		/// Optional associated tag used in the subject when receiving from this user
		/// </summary>
		string Tag { get; set; }
	}
}
