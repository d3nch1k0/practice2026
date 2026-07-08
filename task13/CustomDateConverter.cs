using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace task13
{
    public class CustomDateTimeConverter: JsonConverter<DateTime>
    {
        private const string Format = "yyyy-MM-dd";
        public override DateTime Read(ref Utf8JsonReader reader,  Type typeToConvert, JsonSerializerOptions options)
        {
            string? dateString = reader.GetString();

            bool isSucces = DateTime.TryParseExact(dateString, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date);

            if (isSucces)
            {
                return date;
            }
            else
            {
                throw new JsonException($"Ошибка, другой формат. Ожидался{Format}");
            }
        }
            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(Format, CultureInfo.InvariantCulture));
        }
    }
}
