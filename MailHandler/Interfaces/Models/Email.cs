using System.Collections.Generic;
using System.Net.Mail;

namespace MailHandler.Interfaces.Models
{
	/// <summary>
	/// The Email model
	/// </summary>
	public class Email
	{
		/// <summary>
		/// The From address
		/// </summary>
		public readonly MailAddress From;

		/// <summary>
		/// The To addresses
		/// </summary>
		public readonly IEnumerable<MailAddress> To;

		/// <summary>
		/// Initializes a new instance of the <see cref="Email"/> class.
		/// </summary>
		/// <param name="from">The from address.</param>
		/// <param name="to">The to addresses.</param>
		/// <exception cref="System.ArgumentException">At least 1 to address required</exception>
		public Email(MailAddress from, params MailAddress[] to)
		{
			From = from;
			To = to;

			if (to.Length == 0)
			{
				throw new System.ArgumentException("At least 1 to address required");
			}
		}

		/// <summary>
		/// Gets or sets the subject.
		/// </summary>
		/// <value>
		/// The subject.
		/// </value>
		public string Subject { get; set; }

		/// <summary>
		/// The text contents
		/// </summary>
		public readonly List<Text> TextContents = new List<Text>();

		/// <summary>
		/// The attachments
		/// </summary>
		public readonly List<Attachment> Attachments = new List<Attachment>();
	}
}
