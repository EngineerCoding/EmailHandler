using System.IO;

namespace MailHandler.Interfaces.Models
{
	/// <summary>
	/// Implementation of an Attachment. This is based on a temporary file.
	/// </summary>
	/// <seealso cref="MailHandler.Interfaces.Models.Attachment" />
	public class TemporaryAttachment : Attachment
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TemporaryAttachment"/> class.
		/// </summary>
		/// <param name="data">The data.</param>
		public TemporaryAttachment(byte[] data)
			: this(new MemoryStream(data))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TemporaryAttachment"/> class.
		/// </summary>
		/// <param name="stream">The stream.</param>
		public TemporaryAttachment(Stream stream)
		{
			FilePath = Path.GetTempFileName();

			using (FileStream fileStream = File.OpenWrite(FilePath))
			{
				stream.CopyTo(fileStream);
			}
		}
	}
}
