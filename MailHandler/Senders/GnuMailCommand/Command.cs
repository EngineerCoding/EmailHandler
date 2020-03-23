using System.Collections.Generic;
using System.Diagnostics;

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
			},
		};

		private readonly IEnumerable<Interfaces.Models.Attachment> _attachments;

		public Command(IEnumerable<string> arguments, IEnumerable<Interfaces.Models.Attachment> attachments)
		{
			foreach (string argument in arguments)
			{
				process.StartInfo.ArgumentList.Add(argument);
			}
			_attachments = attachments;
		}

		public void Execute()
		{
			process.Start();
			process.WaitForExit();

			foreach (Interfaces.Models.Attachment attachment in _attachments)
			{
				attachment.Dispose();
			}
		}
	}
}
