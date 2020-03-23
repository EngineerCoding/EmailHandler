using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;

namespace MailHandler.Interfaces.Models
{
	public class Email
	{
		public const string TextContentType = "text/plain";
		public const string HtmlContentType = "text/html";

		public readonly MailAddress From;
		public readonly MailAddress[] To;

		public Email(MailAddress from, params MailAddress[] to)
		{
			From = from;
			To = to;

			if (To.Length == 0)
			{
				throw new System.ArgumentException("At least 1 to address required");
			}
		}

		public string Subject { get; set; }

		public string TextContent { get; set; }
		public string HtmlContent { get; set; }

		public readonly List<Attachment> Attachments = new List<Attachment>();

		public Attachment GetTextContentAsAttachment()
		{
			return AsAttachment(TextContent, TextContentType);
		}

		public Attachment GetHtmlContentAsAttachment()
		{
			return AsAttachment(HtmlContent, HtmlContentType);
		}

		private static Attachment AsAttachment(string content, string contentType)
		{
			if (content == null)
			{
				return null;
			}

			return new TemporaryAttachment(Encoding.UTF8.GetBytes(content))
			{
				ContentType = contentType,
			};
		}
	}
}
