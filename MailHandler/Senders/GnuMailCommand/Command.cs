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
				UseShellExecute = true,
				CreateNoWindow = true,
			},
		};

		private IEnumerable<string> _files;

		public Command(IEnumerable<string> arguments, IEnumerable<string> files)
		{
			foreach (string argument in arguments)
			{
				process.StartInfo.ArgumentList.Add(argument);
			}
			_files = files;
		}

		public void Execute()
		{
			process.Start();
			process.WaitForExit();

			foreach (string file in _files)
			{
				File.Delete(file);
			}
		}
	}
}
