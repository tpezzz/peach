using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Tree.Api.Model {
    public static class Format {
        public static string DateTime = "yyyy-MM-dd'T'HH:mm:ss.fff'Z'";
    }

    public class DateTimeOffsetToStringCoverter : DateTimeConverterBase {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var val = ((DateTimeOffset)value).UtcDateTime.ToString(Format.DateTime);
            writer.WriteValue(val);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            var value = reader.Value;

            if (!(value is DateTime? && ((System.DateTime)(reader.Value)).Kind == DateTimeKind.Utc))
                throw new JsonReaderException("Provided date is not in the UTC format.");

            return new DateTimeOffset(new DateTimeOffset((DateTime)value).UtcDateTime);
        }
    }
}