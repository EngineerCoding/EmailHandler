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
			if (_textAttachment != null)
			{
				arguments.AddRange(_textAttachment.GetArguments());
				_attachments.Add(_textAttachment);
			}

			if (_htmlAttachment != null)
			{
				arguments.AddRange(_htmlAttachment.GetArguments());
				_attachments.Add(_htmlAttachment);
			}

			// Add recipients
			arguments.AddRange(recipientMailAddresses.Select(recipient => recipient.Address));

			System.Console.WriteLine("Executing: mail");
			foreach (string arg in arguments)
			{
				System.Console.WriteLine("\t" + arg);
			}
	
			return new Command(arguments, _attachments);
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
