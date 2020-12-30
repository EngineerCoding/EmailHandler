using System;
using System.IO;

namespace MailHandler.Interfaces.Models
{
	public abstract class Attachment : IDisposable
	{
		public string FilePath { get; set; }

		private string _displayFilename;

		public string DisplayFileName {
			get
			{
				if (_displayFilename == null)
				{
					return Path.GetFileName(FilePath);
				}
				return _displayFilename;
			}
			set
			{
				_displayFilename = value;
			}
		}

		public string ContentType { get; set; } = "application/octet-stream";

		public string Encoding { get; set; }

		public bool AllowFileDisposal { get; set; }

		public void Dispose()
		{
			if (File.Exists(FilePath) && AllowFileDisposal)
			{
				File.Delete(FilePath);
			}
		}
	}
}
