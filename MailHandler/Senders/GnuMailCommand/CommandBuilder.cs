using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace MailHandler.Senders.GnuMailCommand
{
	public class CommandBuilder
	{
		private const string SubjectFormat = "--subject={0}";
		private const string ReturnAddressFormat = "--return-address={0}";
		private const string Alternative = "--alternative";
		private const string QuotedPrintableEncoding = "quoted-printable";

		private string _subject;
		public string Subject
		{
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					_subject = string.Format(SubjectFormat, Escape(value));
				}
			}
		}

		private string _returnAddress;
		public MailAddress ReturnAddress
		{
			set
			{
				_returnAddress = string.Format(ReturnAddressFormat, value.Address);
			}
		}

		private Interfaces.Models.Attachment _textAttachment;
		public Interfaces.Models.Attachment TextContent
		{
			set
			{
				_textAttachment = value;
			}
		}

		private Interfaces.Models.Attachment _htmlAttachment;
		public Interfaces.Models.Attachment HtmlContent
		{
			set
			{
				_htmlAttachment = value;
			}
		}

		private List<Interfaces.Models.Attachment> _attachments = new List<Interfaces.Models.Attachment>();

		public void AddAttachment(Interfaces.Models.Attachment attachment)
		{
			if (attachment != null)
			{
				_attachments.Add(attachment);
			}
		}

		public void AddAttachments(IEnumerable<Interfaces.Models.Attachment> attachments)
		{
			foreach (Interfaces.Models.Attachment attachment in attachments)
			{
				AddAttachment(attachment);
			}
		}

		public Command Build(IEnumerable<MailAddress> recipients)
		{
			List<MailAddress> recipientMailAddresses = recipients.Where(recipient => recipient != null).ToList();
			if (recipientMailAddresses.Count == 0)
			{
				throw new System.ArgumentException("At least 1 argument required");
			}
   
			List<string> arguments = new List<string>();
			if (!string.IsNullOrEmpty(_subject)) arguments.Add(_subject);
			if (!string.IsNullOrEmpty(_returnAddress)) arguments.Add(_returnAddress);

			// Add attachments
			foreach (Interfaces.Models.Attachment attachment in _attachments)
			{
				arguments.AddRange(attachment.GetArguments());
			}

			// Add alternative attachments
			arguments.Add(Alternative);

			string inputStreamPath = null;
			if (_textAttachment != null)
			{
				inputStreamPath = _textAttachment.FilePath;
				_attachments.Add(_textAttachment);
			}

			if (_htmlAttachment != null)
			{
				if (inputStreamPath == null)
				{
					inputStreamPath = _htmlAttachment.FilePath;
				}
				else
				{
					_htmlAttachment.Encoding = QuotedPrintableEncoding;
					arguments.AddRange(_htmlAttachment.GetArguments());
				}

				_attachments.Add(_htmlAttachment);
			}

			// Add recipients
			arguments.AddRange(recipientMailAddresses.Select(recipient => recipient.Address));
	
			return new Command(arguments, _attachments, inputStreamPath);
		}

		private static readonly char[] UnsafeCharacters = new char[]
		{
			'"', '$',
		};

		private static string Escape(string unescaped)
		{
			foreach (char unsafeChar in UnsafeCharacters)
			{
				string representation = unsafeChar.ToString();
				unescaped = unescaped.Replace(representation, "\\" + representation);
			}
			return unescaped;
		}
	}
}
