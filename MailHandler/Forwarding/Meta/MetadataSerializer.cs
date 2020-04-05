using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;

namespace MailHandler.Forwarding.Meta
{
	public class MetadataSerializer
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
			yield break;
		}

		public static string SerializeWithText(Metadata metadata, string body)
		{
			if (string.IsNullOrEmpty(body))
			{
				return null;
			}

			StringBuilder stringBuilder = new StringBuilder();
			foreach ((string key, string value) in EnumerateProperties(metadata))
			{
				stringBuilder
					.Append(string.Format(PropertyFormat, key, value))
					.Append("\n");
			}
			stringBuilder
				.Append(Divider)
				.Append(body);
			return stringBuilder.ToString();
		}

		public static string SerializeWithHtml(Metadata metadata, string body)
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
