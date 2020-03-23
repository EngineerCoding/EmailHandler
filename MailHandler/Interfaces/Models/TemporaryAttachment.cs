using System.IO;

namespace MailHandler.Interfaces.Models
{
	public class TemporaryAttachment : Attachment
	{
		public TemporaryAttachment(byte[] data) : this(new MemoryStream(data))
		{ }

		public TemporaryAttachment(Stream stream)
		{
			FilePath = Path.GetTempFileName();
			AllowFileDisposal = true;

			using (FileStream fileStream = File.OpenWrite(FilePath))
			{
				stream.CopyTo(fileStream);
			}
		}
	}
}
