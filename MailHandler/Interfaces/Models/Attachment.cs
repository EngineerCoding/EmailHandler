using System;
using System.IO;

namespace MailHandler.Interfaces.Models
{
	/// <summary>
	/// An abstract attachment
	/// </summary>
	/// <seealso cref="System.IDisposable" />
	public abstract class Attachment : IDisposable
	{
		/// <summary>
		/// Gets or sets the file path.
		/// </summary>
		/// <value>
		/// The file path.
		/// </value>
		public string FilePath { get; set; }

		private string _displayFilename;

		/// <summary>
		/// Gets or sets the display name of the file.
		/// </summary>
		/// <value>
		/// The display name of the file.
		/// </value>
		public string DisplayFileName {
			get
			{
				if (_displayFilename == null)
				{
					return Path.GetFileName(FilePath);
				}
				return _displayFilename;
			}
			set => _displayFilename = value;
		}

		/// <summary>
		/// Gets or sets the type of the content.
		/// </summary>
		/// <value>
		/// The type of the content.
		/// </value>
		public string ContentType { get; set; } = "application/octet-stream";

		/// <summary>
		/// Gets or sets the encoding.
		/// </summary>
		/// <value>
		/// The encoding.
		/// </value>
		public string Encoding { get; set; }

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (File.Exists(FilePath))
				{
					File.Delete(FilePath);
				}
			}
		}

		/// <inheritdoc/>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
