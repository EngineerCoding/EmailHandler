namespace MailHandler.Interfaces.Models
{
	/// <summary>
	/// A model for a piece of text content
	/// </summary>
	public class Text
	{
		/// <summary>
		/// Gets or sets the raw text.
		/// </summary>
		/// <value>
		/// The raw text.
		/// </value>
		public string RawText { get; set; }

		/// <summary>
		/// Gets or sets the type of the MIME.
		/// </summary>
		/// <value>
		/// The type of the MIME.
		/// </value>
		public string MimeType { get; set; }

		/// <summary>
		/// Gets or sets the charset.
		/// </summary>
		/// <value>
		/// The charset.
		/// </value>
		public string Charset { get; set; }
	}
}
