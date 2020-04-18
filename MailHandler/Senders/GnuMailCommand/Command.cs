using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MailHandler.Senders.GnuMailCommand
{
	public class Command
	{
		private readonly Process process = new Process
		{
			StartInfo = new ProcessStartInfo("/usr/bin/mail")
			{
				UseShellExecute = false,
				CreateNoWindow = true,
				RedirectStandardInput = true,
			},
		};

		private readonly IEnumerable<Interfaces.Models.Attachment> _attachments;
		private readonly string _standardInputStreamPath;

		public Command(IEnumerable<string> arguments, IEnumerable<Interfaces.Models.Attachment> attachments, string standardInputStreamPath)
		{
			foreach (string argument in arguments)
			{
				process.StartInfo.ArgumentList.Add(argument);
			}
			_attachments = attachments;
			_standardInputStreamPath = standardInputStreamPath;
		}

		public void Execute()
		{
			process.Start();

			if (_standardInputStreamPath != null)
			{
				using (FileStream fileStream = File.OpenRead(_standardInputStreamPath))
				using (StreamReader reader = new StreamReader(fileStream))
				{
					process.StandardInput.Write(reader.ReadToEnd());
				}
			}

			process.StandardInput.Close();
			process.WaitForExit();

			foreach (Interfaces.Models.Attachment attachment in _attachments)
			{
				attachment.Dispose();
			}
		}
	}
}
