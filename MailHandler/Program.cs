using Autofac;
using CommandLine;
using MailDatabase;
using MailDatabase.SqlLite;
using MailHandler.Forwarding;
using MailHandler.Interfaces;
using MailHandler.Senders;
using System;

namespace MailHandler
{
	public class Program
	{
		private const int VALID_RUN = 0;
		private const int INVALID_RUN = 1;
		private const int INVALID_ARGUMENTS = 2;

		public static IContainer Resolver { get; private set; }

		public static int Main(string[] args)
		{
			return Parser.Default.ParseArguments<Options>(args)
				.MapResult(Run, (_) => INVALID_ARGUMENTS);
		}

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

			Resolver = InitDependencyInjection(options);
			IEmailHandler handler = Resolver.Resolve<IEmailHandler>();
			if (!handler.HandleIncomingEmail())
			{
				return INVALID_RUN;
			}

			return VALID_RUN;
		}

		private static IContainer InitDependencyInjection(Options options)
		{
			ContainerBuilder containerBuilder = new ContainerBuilder();
			containerBuilder.RegisterInstance(options);
			containerBuilder.RegisterType<MailKitSender>().As<IEmailSender>();
			containerBuilder.RegisterType<Forwarder>().As<IEmailHandler>();
			containerBuilder.RegisterInstance(new EmailDatabase(options.EmailDatabase)).As<IEmailDatabase>();

			return containerBuilder.Build();
		}
	}
}
