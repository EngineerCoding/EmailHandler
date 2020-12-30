using CommandLine;

namespace MailHandler
{
	public class Options
	{
		[Option(HelpText = "The file to read settings from in JSON format")]
		public string SettingsFile { get; set; }

		[Option(HelpText = "Email address where incoming emails are sent to")]
		public string RelayEmail { get; set; }

		[Option(HelpText = "Read an email from a file")]
		public string EmailFile { get; set; }

		[Option(HelpText = "Read an email from stdin")]
		public bool StdIn { get; set; }

		[Option(HelpText = "The location where the database is stored for blacklisting")]
		public string EmailDatabase { get; set; }

		#region SMTP

		[Option(HelpText = "The host this client lives on")]
		public string SmtpHost { get; set; }

		[Option(HelpText = "The port of the SMTP server")]
		public ushort SmtpPort { get; set; } = 25;

		[Option(HelpText = "The user which is sends the emails to the RelayEmail")]
		public string SmtpUser { get; set; }

		#endregion
	}
}
