using System;
using Newtonsoft.Json;

 namespace SmartcatPlugin.Models.Smartcat
 {
	public class ExternalObjectIdConverter : JsonConverter<ExternalObjectId>
	{
		public override void WriteJson(
			JsonWriter writer,
			ExternalObjectId value,
			JsonSerializer serializer)
		{
			serializer.Serialize(writer, value.ToString());
		}

		public override ExternalObjectId ReadJson(
			JsonReader reader,
			Type objectType,
			ExternalObjectId existingValue,
			bool hasExistingValue,
			JsonSerializer serializer)
		{
			var s = (string)reader.Value;
			return ExternalObjectId.Parse(s);
		}
	}
}