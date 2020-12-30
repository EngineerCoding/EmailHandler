namespace MailHandler.Forwarding.Meta
{
	/// <summary>
	/// The metadata object
	/// </summary>
	public class Metadata
	{
		public string To { get; set; }
		public string From { get; set; }
		public string Cc { get; set; }
		public string Bcc { get; set; }
		public string MessageId { get; set; }
		public string ReplyTo { get; set; }
		public string Tag { get; set; }
	}
}
