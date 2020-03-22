using MailHandler.Forwarding.Meta;
using MailHandler.Interfaces;
using MimeKit;

namespace MailHandler.Forwarding
{
	public class Forwarder : IEmailHandler
	{
		private readonly Options _options;
		private readonly IEmailSender _sender;

		public Forwarder(Options options, IEmailSender emailSender)
		{
			_options = options;
			_sender = emailSender;
		}

		public bool HandleIncomingEmail()
		{
			MimeParser mimeParser = new MimeParser(_options.GetInputEmail(), !_options.StdIn);
			MimeMessage message = mimeParser.ParseMessage();
			if (IsFromRelay(message.From[0].ToString()))
			{
				// Not supported yet
				return false;
			}
			else
			{
				ForwardEmail(message);
			}
			
			return true;
		}

		public void ForwardEmail(MimeMessage mimeMessage)
		{
			Metadata metadata = MetadataFactory.GenerateFrom(_options, mimeMessage);
			string textBody = mimeMessage.GetTextBody(MimeKit.Text.TextFormat.Plain);

			_sender.SendEmail(
				_options.GetSender(), _options.RelayEmail, mimeMessage.Subject,
				MetadataSerializer.Serialize(metadata, textBody),
				mimeMessage.GetTextBody(MimeKit.Text.TextFormat.Html));
		}

		public bool IsFromRelay(string email)
		{
			return string.Equals(_options.RelayEmail, email, System.StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
