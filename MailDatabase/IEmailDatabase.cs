using System.Collections.Generic;
using System.Linq;

namespace MailDatabase
{
	/// <summary>
	/// Interface for the email database
	/// </summary>
	public interface IEmailDatabase
	{
		/// <summary>
		/// Attempts to find the entry for the given username
		/// </summary>
		/// <param name="emailUser">The username (before '@' part)</param>
		/// <returns>
		/// The associated entry or null
		/// </returns>
		IEmailEntry Find(string emailUser);

		/// <summary>
		/// Inserts an entry for an username
		/// </summary>
		/// <param name="emailUser">The username (before '@' part)</param>
		void Insert(IEmailEntry emailEntry);

		/// <summary>
		/// Updates an entry
		/// </summary>
		/// <param name="emailUser">The username (before '@' part)</param>
		void Update(IEmailEntry emailEntry);

		/// <summary>
		/// Deletes an entry
		/// </summary>
		/// <param name="emailUser">The username (before '@' part)</param>
		void Delete(IEmailEntry emailEntry);

		/// <summary>
		/// Retrieves all entries
		/// </summary>
		/// <param name="skip">The optional skip.</param>
		/// <param name="take">The optional take.</param>
		/// <returns>
		/// All available email entries
		/// </returns>
		IEnumerable<IEmailEntry> All(int? skip = null, int? take = null);

		/// <summary>
		/// The amount of email entries
		/// </summary>
		/// <returns>
		/// The amount of available email entries
		/// </returns>
		int GetCount();
	}
}
