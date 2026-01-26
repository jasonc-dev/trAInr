using System.Text.Json;
using System.Text.Json.Serialization;

namespace trAInr.Application.DTOs.AI;

/// <summary>
/// JSON converter that handles both string and integer values for int properties.
/// Useful when deserializing responses from external APIs or AI services that may
/// return numbers as strings.
/// </summary>
public class StringToIntConverter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            if (int.TryParse(stringValue, out var result))
            {
                return result;
            }
            throw new JsonException($"Unable to convert '{stringValue}' to Int32.");
        }
        
        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetInt32();
        }
        
        throw new JsonException($"Unexpected token type: {reader.TokenType}");
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}

/// <summary>
/// JSON converter that handles both string and integer values for nullable int properties.
/// Useful when deserializing responses from external APIs or AI services that may
/// return numbers as strings.
/// </summary>
public class StringToNullableIntConverter : JsonConverter<int?>
{
    public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }
        
        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return null;
            }
            
            if (int.TryParse(stringValue, out var result))
            {
                return result;
            }
            throw new JsonException($"Unable to convert '{stringValue}' to Int32.");
        }
        
        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetInt32();
        }
        
        throw new JsonException($"Unexpected token type: {reader.TokenType}");
    }

    public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteNumberValue(value.Value);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
