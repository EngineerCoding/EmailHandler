using System.Collections.Generic;
using System.Net.Mail;

namespace MailHandler.Interfaces.Models
{
	public class Email
	{
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

		public readonly List<Text> Texts = new List<Text>();

		public readonly List<Attachment> Attachments = new List<Attachment>();
	}
}
