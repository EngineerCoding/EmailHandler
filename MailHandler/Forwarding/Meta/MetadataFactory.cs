using MimeKit;
using System.Collections.Generic;
using System.Linq;

namespace MailHandler.Forwarding.Meta
{
	/// <summary>
	/// The factory for creating the <seealso cref="Metadata"/> object
	/// </summary>
	public static class MetadataFactory
	{
		/// <summary>
		/// Generates metadata from a MimeMessage.
		/// </summary>
		/// <param name="mimeMessage">The MIME message.</param>
		/// <returns>
		/// The Metadata
		/// </returns>
		public static Metadata GenerateFrom(MimeMessage mimeMessage)
		{
			Metadata metadata = new Metadata
			{
				From = ToReadableString(mimeMessage.From),
				To = ToReadableString(mimeMessage.To),
				Cc = ToReadableString(mimeMessage.Cc),
				Bcc = ToReadableString(mimeMessage.Bcc),
			};

			if (mimeMessage.Headers.Contains(HeaderId.MessageId))
			{
				metadata.MessageId = mimeMessage.Headers[HeaderId.MessageId];
			}
			return metadata;
		}

		/// <summary>
		/// Converts to an email address to a readable string.
		/// </summary>
		/// <param name="internetAddresses">The internet addresses.</param>
		/// <returns>
		/// The readable string
		/// </returns>
		private static string ToReadableString(IEnumerable<InternetAddress> internetAddresses)
		{
			IEnumerable<string> addresses = internetAddresses.Select(address => address.ToString());
			return string.Join(',', addresses);
		}
	}
}
