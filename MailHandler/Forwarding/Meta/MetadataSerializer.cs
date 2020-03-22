using System.Reflection;
using System.Text;

namespace MailHandler.Forwarding.Meta
{
	public class MetadataSerializer
	{
		private const string PropertyFormat = "{0}: {1}";
		private const string Divider = "----------------------------";

		public static string Serialize(Metadata metadata)
		{
			StringBuilder stringBuilder = new StringBuilder();
			PropertyInfo[] properties = typeof(Metadata).GetProperties();
			foreach (PropertyInfo property in properties)
			{
				stringBuilder.Append(
					string.Format(PropertyFormat, property.Name, property.GetValue(metadata)));
				stringBuilder.Append("\n");
			}
			return stringBuilder.ToString();
		}

		public static string Serialize(Metadata metadata, string body)
		{
			return Serialize(metadata) + Divider + "\n" + body;
		}
	}
}
