using Newtonsoft.Json;
using System.IO;
using System.Net.Mail;

namespace MailHandler
{
	public static class OptionsExtensions
	{
		public const string FileDoesNotExist = "File '{0}' does not exist";
		public const string RelayEmailNotSet = "The relay email must be set!";
		public const string InvalidEmailAddress = "The e-mail address '{0}' is not valid";
		public const string ExpectedEmailFile = "Expected emailfile setting";
		public const string ExpectedSmtpHostSetting = "Expected SMTP host setting";
		public const string ExpectedSmtpUserSetting = "Expected SMTP user setting";

		/// <summary>
		/// Loads the settings from a JSON file
		/// </summary>
		/// <param name="options">The options to populate</param>
		/// <returns>Whether the options are in a correct state. When true, either loading succeeded or no settings file is available.</returns>
		public static bool LoadSettingsFile(this Options options)
		{
			if (options.SettingsFile != null)
			{
				if (!File.Exists(options.SettingsFile))
				{
					return false;
				}

				Options newOptions = JsonConvert.DeserializeObject<Options>(File.ReadAllText(options.SettingsFile));
				options.RelayEmail = newOptions.RelayEmail;
				options.EmailFile = newOptions.EmailFile;
				options.StdIn = newOptions.StdIn;
			}
			return true;
		}

		/// <summary>
		/// Checks whether the settings are valid as is
		/// </summary>
		/// <param name="options">The options to check</param>
		/// <returns>A string with an error or null when valid</returns>
		public static string Validate(this Options options)
		{
			// Check the relay email
			if (string.IsNullOrEmpty(options.RelayEmail))
			{
				return RelayEmailNotSet;
			}

			if (!IsValidEmail(options.RelayEmail))
			{
				return string.Format(InvalidEmailAddress, options.RelayEmail);
			}

			// Check the email file, if selected to use
			if (!options.StdIn)
			{
				if (string.IsNullOrEmpty(options.EmailFile))
				{
					return ExpectedEmailFile;
				}
				else if (!File.Exists(options.EmailFile))
				{
					return string.Format("emailfile: " + FileDoesNotExist, options.EmailFile);
				}
			}

			// Check Smtp settings
			if (string.IsNullOrEmpty(options.SmtpHost))
			{
				return ExpectedSmtpHostSetting;
			}

			if (string.IsNullOrEmpty(options.SmtpUser))
			{
				return ExpectedSmtpUserSetting;
			}

			return null;
		}

		/// <summary>
		/// Retrieves the correct input stream which represents the email to process.
		/// This either can be from the StandardInput or from the Email File.
		/// </summary>
		/// <param name="options"></param>
		/// <returns></returns>
		public static Stream GetInputEmail(this Options options)
		{
			if (options.StdIn)
			{
				return System.Console.OpenStandardInput();
			}
			return File.OpenRead(options.EmailFile);
		}

		/// <summary>
		/// Retrieves the fully qualified email address which acts as sender
		/// </summary>
		/// <param name="options"></param>
		/// <returns></returns>
		public static string GetSender(this Options options)
		{
			return string.Format("{0}@{1}", options.SmtpUser, options.SmtpHost);
		}

		/// <summary>
		/// Checks whether the given email address is a valid email address (based on the format, not on the validity)
		/// </summary>
		/// <param name="email">The email address to check</param>
		/// <returns>Whether the email address is in a valid format</returns>
		private static bool IsValidEmail(string email)
		{
			try
			{
				MailAddress mailAddress = new MailAddress(email);
				return mailAddress.Address == email;
			}
			catch
			{
				return false;
			}
		}
	}
}
