using MimeKit;

namespace MailHandler.Forwarding
{
	public static class ContentEncodingExtensions
	{
		private const string Base64 = "base64";
		private const string Binary = "binary";
		private const string QuotedPrintable = "quoted-printable";
		private const string EightBit = "8bit";
		private const string SevenBit = "7bit";

		public static string GetRaw(this ContentEncoding contentEncoding)
		{
			switch (contentEncoding)
			{
				case ContentEncoding.Base64:
					return Base64;
				case ContentEncoding.Binary:
					return Binary;
				case ContentEncoding.QuotedPrintable:
					return QuotedPrintable; 
				case ContentEncoding.EightBit:
					return EightBit;
				case ContentEncoding.SevenBit:
					return SevenBit;
				default:
					return null;
			}
		}
	}
}
