using System.Collections.Generic;
using System.IO;

namespace MailHandler.Senders.GnuMailCommand
{
	public class CommandBuilder
	{
		private const string Subject = "\"--subject={0}\"";
		private const string ReturnAddress = "--return-address={0}";
		private const string ContentType = "--content-type={0}";
		private const string Attach = "--attach={0}";
		private const string Alternative = "--alternative";

		private string subject;
		private string returnAddress;
		private readonly List<string> contentArguments = new List<string>();
		private readonly List<string> contentFiles = new List<string>();

		public CommandBuilder SetSubject(string subject)
		{
			if (!string.IsNullOrEmpty(subject))
			{
				this.subject = string.Format(Subject, Escape(subject));
			}
			return this;
		}

		public CommandBuilder SetFrom(string from)
		{
			if (!string.IsNullOrEmpty(from))
			{
				returnAddress = string.Format(ReturnAddress, from);
			}
			return this;
		}

		public void AttachContent(ContentType contentType, string content)
		{
			if (!string.IsNullOrEmpty(content))
			{
				string path = Path.GetTempFileName();
				File.WriteAllText(path, content);

				contentArguments.Add(string.Format(ContentType, "text/" + contentType.ToString().ToLowerInvariant()));
				contentArguments.Add(string.Format(Attach, path));
				contentFiles.Add(path);
			}
		}

		public Command Build(IEnumerable<string> recipients)
		{
			List<string> arguments = new List<string>
			{
				Alternative,
			};

			if (!string.IsNullOrEmpty(subject)) arguments.Add(subject);
			if (!string.IsNullOrEmpty(returnAddress)) arguments.Add(returnAddress);
			arguments.AddRange(contentArguments);
			arguments.AddRange(recipients);

			return new Command(arguments, contentFiles);
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
