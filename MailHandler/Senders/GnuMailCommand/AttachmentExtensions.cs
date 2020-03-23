using MailHandler.Interfaces.Models;
using System.Collections.Generic;

namespace MailHandler.Senders.GnuMailCommand
{
	public static class AttachmentExtensions
	{
		private const string ContentFileNameFormat = "--content-filename={0}";
		private const string ContentTypeFormat = "--content-type={0}";
		private const string ContentEncodingFormat = "--encoding={0}";
		private const string AttachFormat = "--attach={0}";

		public static IEnumerable<string> GetArguments(this Attachment attachment)
		{
			List<string> components = new List<string>(4)
			{
				string.Format(ContentFileNameFormat, attachment.DisplayFileName),
				string.Format(ContentTypeFormat, attachment.ContentType),
			};

			if (!string.IsNullOrEmpty(attachment.Encoding))
			{
				components.Add(string.Format(ContentEncodingFormat, attachment.Encoding));
			}
			components.Add(string.Format(AttachFormat, attachment.FilePath));
			return components;
		}
	}
}
