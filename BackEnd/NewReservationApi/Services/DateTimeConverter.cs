using System.Text.Json;

namespace NewReservationApi.Services;
public class DateTimeConverter : System.Text.Json.Serialization.JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            if (DateTime.TryParse(reader.GetString(), out DateTime dateTime))
            {
                return dateTime;
            }
            else
            {
                throw new JsonException($"Unable to parse '{reader.GetString()}' as a valid DateTime.");
            }
        }
        else
        {
            throw new JsonException($"Expected a string token, but found '{reader.TokenType}'.");
        }
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var dt = value.ToUniversalTime();
        writer.WriteStringValue($"{dt:yyyy-MM-dd}T{dt:HH:mm}:00Z");
    }
}
