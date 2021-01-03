using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IdentityControl.API.Extensions
{
    public static class JsonExtensions
    {
        private static readonly JsonSerializerSettings _jsonOptions = new JsonSerializerSettings
        {
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            Converters = new List<JsonConverter>
                {new UtcDateTimeConverter(), new TrimmingStringConverter(), new StringEnumConverter()}
        };

        public static T FromJson<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _jsonOptions);
        }

        public static string ToJson<T>(this T obj)
        {
            return JsonConvert.SerializeObject(obj, _jsonOptions);
        }

        public class UtcDateTimeConverter : IsoDateTimeConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(DateTime) || objectType == typeof(DateTime?);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteValue(string.Format("{0:yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFF}Z", Convert.ToDateTime(value)));
            }
        }

        public class StringEnumCustomConverter : StringEnumConverter
        {
            public override bool CanRead => true;
            public override bool CanWrite => true;
        }

        public class TrimmingStringConverter : JsonConverter
        {
            public override bool CanRead => true;

            public override bool CanWrite => false;

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(string);
            }

            public override object ReadJson(
                JsonReader reader,
                Type objectType,
                object existingValue,
                JsonSerializer serializer)
            {
                return reader.Value is string str ? str.Trim() : reader.Value;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}