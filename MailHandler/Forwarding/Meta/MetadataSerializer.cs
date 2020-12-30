using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;

namespace MailHandler.Forwarding.Meta
{
	/// <summary>
	/// A class which serialized the Metadata in email text
	/// </summary>
	public static class MetadataSerializer
	{
		private const string PropertyFormat = "{0}: {1}";
		private const string Divider = "----------------------------";

		private const string TableOpening = "<table>";
		private const string TableClosing = "</table>";
		private const string TrOpening = "<tr>";
		private const string TrClosing = "</tr>";
		private const string TdOpening = "<td>";
		private const string TdClosing = "</td>";
		private const string Br = "<br/>";

		/// <summary>
		/// Enumerates the properties.
		/// </summary>
		/// <param name="metadata">The metadata.</param>
		/// <returns>
		/// An enumerable of tuples containing the property name and value
		/// </returns>
		private static IEnumerable<(string, string)> EnumerateProperties(Metadata metadata)
		{
			PropertyInfo[] properties = typeof(Metadata).GetProperties();
			foreach (PropertyInfo property in properties)
			{
				string value = property.GetValue(metadata)?.ToString();
				if (!string.IsNullOrWhiteSpace(value))
				{
					yield return (property.Name, value);
				}
			}
		}

		/// <summary>
		/// Serializes for plain text.
		/// </summary>
		/// <param name="metadata">The metadata.</param>
		/// <param name="body">The body.</param>
		/// <returns>
		/// The metadata prepended body
		/// </returns>
		public static string SerializeForText(Metadata metadata, string body)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach ((string key, string value) in EnumerateProperties(metadata))
			{
				stringBuilder
					.Append(string.Format(PropertyFormat, key, value))
					.AppendLine();
			}
			stringBuilder
				.Append(Divider)
				.AppendLine()
				.Append(body);
			return stringBuilder.ToString();
		}

		/// <summary>
		/// Serializes for HTML.
		/// </summary>
		/// <param name="metadata">The metadata.</param>
		/// <param name="body">The body.</param>
		/// <returns>
		/// The body with HTML prepended Metadata
		/// </returns>
		public static string SerializeForHtml(Metadata metadata, string body)
		{
			if (string.IsNullOrEmpty(body))
			{
				return null;
			}

			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(TableOpening);
			foreach ((string key, string value) in EnumerateProperties(metadata))
			{
				stringBuilder
					.Append(TrOpening)
						.Append(TdOpening)
							.Append(key)
						.Append(TdClosing)
						.Append(TdOpening)
							.Append(WebUtility.HtmlEncode(value))
						.Append(TdClosing)
					.Append(TrClosing);
			}

			stringBuilder
				.Append(TableClosing)
				.Append(Br)
				.Append(body);
			return stringBuilder.ToString();
		}
	}
}
