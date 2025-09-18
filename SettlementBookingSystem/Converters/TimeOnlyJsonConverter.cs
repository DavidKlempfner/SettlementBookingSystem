using System.Text.Json;
using System.Text.Json.Serialization;

namespace SettlementBookingSystem.Converters
{
    public class TimeOnlyJsonConverter : JsonConverter<TimeOnly>
    {
        private const string TimeFormat = "HH:mm";

        /// <summary>
        /// Reads and converts the JSON string representation of a time to a <see cref="TimeOnly"/> object.
        /// </summary>
        /// <remarks>The expected time format is specified by the <c>TimeFormat</c> field (e.g., "HH:mm").
        /// If the input does not match this format, a <see cref="JsonException"/> is thrown which results in a BadRequest being returned to the API caller.</remarks>
        /// <param name="reader">The <see cref="Utf8JsonReader"/> to read the JSON data from.</param>
        /// <param name="typeToConvert">The type of the object to convert. This parameter is ignored in this implementation.</param>
        /// <param name="options">The serializer options to use. This parameter is ignored in this implementation.</param>
        /// <returns>A <see cref="TimeOnly"/> object representing the parsed time.</returns>
        /// <exception cref="JsonException">Thrown if the JSON value is null, empty, or whitespace, or if the value does not match the expected time
        /// format.</exception>
        public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var timeString = reader.GetString();

            if (string.IsNullOrWhiteSpace(timeString))
            {
                throw new JsonException("Booking time cannot be empty.");
            }

            if (!TimeOnly.TryParseExact(timeString, TimeFormat, out var time))
            {
                throw new JsonException($"Invalid time format. Expected format is '{TimeFormat}' (e.g., '09:30').");
            }

            return time;
        }

        public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(TimeFormat));
        }
    }
}
