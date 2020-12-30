using CommandLine;
using MailDatabase;
using MailDatabase.SqlLite;
using MailHandler.Forwarding;
using MailHandler.Interfaces;
using MailHandler.Senders;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MailHandler
{
	/// <summary>
	/// The entrypoint class
	/// </summary>
	public static class Program
	{
		private const int VALID_RUN = 0;
		private const int INVALID_RUN = 1;
		private const int INVALID_ARGUMENTS = 2;

		/// <summary>
		/// The entry point method
		/// </summary>
		/// <param name="args">The arguments.</param>
		/// <returns>
		/// The program return code
		/// </returns>
		public static int Main(string[] args)
		{
			return Parser.Default.ParseArguments<Options>(args)
				.MapResult(Run, (_) => INVALID_ARGUMENTS);
		}

		/// <summary>
		/// Runs based on the specified options.
		/// </summary>
		/// <param name="options">The options.</param>
		/// <returns>
		/// The program return code
		/// </returns>
		private static int Run(Options options)
		{
			if (!options.LoadSettingsFile())
			{
				Console.Error.WriteLine(string.Format(OptionsExtensions.FileDoesNotExist, options.SettingsFile));
				return INVALID_RUN;
			}

			string error = options.Validate();
			if (!string.IsNullOrEmpty(error))
			{
				Console.Error.WriteLine(error);
				return INVALID_ARGUMENTS;
			}

			IServiceProvider serviceProvider = InitDependencyInjection(options);
			IEmailHandler handler = serviceProvider.GetRequiredService<IEmailHandler>();
			if (!handler.HandleIncomingEmail())
			{
				return INVALID_RUN;
			}

			return VALID_RUN;
		}

		private static IServiceProvider InitDependencyInjection(Options options)
		{
			IServiceCollection services = new ServiceCollection();
			services.AddSingleton(options);
			services.AddSingleton<IEmailSender, MailKitSender>();
			services.AddSingleton<IEmailHandler, Forwarder>();
			services.AddSingleton<IEmailDatabase>(new EmailDatabase(options.EmailDatabase));
			return services.BuildServiceProvider();
		}
	}
}
