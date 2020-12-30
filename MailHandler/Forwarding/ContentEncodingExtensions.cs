using MimeKit;
using System;

namespace MailHandler.Forwarding
{
	/// <summary>
	/// Extensions for the content encoding
	/// </summary>
	public static class ContentEncodingExtensions
	{
		private const string Base64 = "base64";
		private const string Binary = "binary";
		private const string QuotedPrintable = "quoted-printable";
		private const string EightBit = "8bit";
		private const string SevenBit = "7bit";

		/// <summary>
		/// Gets the raw encoding as a string.
		/// </summary>
		/// <param name="contentEncoding">The content encoding.</param>
		/// <returns>
		/// The raw encoding as a string
		/// </returns>
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
					throw new NotImplementedException("Raw content encoding: " + contentEncoding.ToString());
			}
		}

		/// <summary>
		/// Gets the content encoding.
		/// </summary>
		/// <param name="contentEncoding">The content encoding.</param>
		/// <returns>
		/// The enum content encoding based on the string
		/// </returns>
		public static ContentEncoding GetContentEncoding(this string contentEncoding)
		{
			switch (contentEncoding)
			{
				case Base64:
					return ContentEncoding.Base64;
				case Binary:
					return ContentEncoding.Binary;
				case QuotedPrintable:
					return ContentEncoding.QuotedPrintable;
				case EightBit:
					return ContentEncoding.EightBit;
				case SevenBit:
					return ContentEncoding.SevenBit;
				default:
					throw new NotImplementedException("String content encoding: " + contentEncoding);
			}
		}
	}
}
