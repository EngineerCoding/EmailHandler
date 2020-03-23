using MimeKit;
using System.Collections.Generic;
using System.Linq;

namespace MailHandler.Forwarding.Meta
{
	public class MetadataFactory
	{
		public static Metadata GenerateFrom(MimeMessage mimeMessage)
		{
			Metadata metadata = new Metadata
			{
				From = ToReadableString(mimeMessage.From),
				To = ToReadableString(mimeMessage.To)
			};

			if (mimeMessage.Headers.Contains(HeaderId.MessageId))
			{
				metadata.MessageId = mimeMessage.Headers[HeaderId.MessageId];
			}
			return metadata;
		}

		private static string ToReadableString(IEnumerable<InternetAddress> internetAddresses)
		{
			IEnumerable<string> addresses = internetAddresses.Select(address => address.ToString());
			return string.Join(',', addresses);
		}
		
	}
}
