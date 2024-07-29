using System;
using Newtonsoft.Json;

namespace SmartcatPlugin.Models.Smartcat
{
	[JsonConverter(typeof(ExternalObjectIdConverter))]
	public class ExternalObjectId : IEquatable<ExternalObjectId>
	{
		public ExternalObjectId()
		{

		}
		public ExternalObjectId(string externalId, string externalType)
		{
			ExternalId = externalId ?? throw new ArgumentNullException(nameof(externalId));
			ExternalType = externalType;
		}

		public static ExternalObjectId Root =>
			new ExternalObjectId("root", null);

		public override string ToString()
		{
			return string.IsNullOrEmpty(ExternalType)
				? $"{ExternalId}"
				: $"{ExternalId};{ExternalType}";
		}

		[JsonProperty("externalId")]
		public string ExternalId { get;  set; }

		[JsonProperty("externalType")]
		public string ExternalType { get;  set; }

		public static ExternalObjectId Parse(string s)
		{
			if (s == null)
				throw new ArgumentNullException(nameof(s));
			var externalObjectId = TryParse(s);
			if (externalObjectId == null)
				throw new FormatException(string.Format("String '{0}' is not a valid DataObjectId", s));
			return externalObjectId;
		}

		public static ExternalObjectId TryParse(string s)
		{
			if (String.IsNullOrEmpty(s))
				return null;
			if (s == "root")
				return Root;
			var splitted = s.Split(';');
			if (splitted.Length == 1)
				return new ExternalObjectId(splitted[0], null);
			if (splitted.Length == 2)
				return new ExternalObjectId(splitted[0], splitted[1]);
			return null;
		}

		public bool Equals(ExternalObjectId other)
		{
			return ToString().Equals(other.ToString());
		}

		public override bool Equals(object obj)
		{
			return obj is ExternalObjectId other && Equals(other);
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}
	}
}