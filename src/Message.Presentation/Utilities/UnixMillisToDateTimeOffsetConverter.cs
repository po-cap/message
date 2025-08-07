using System.Text.Json;
using System.Text.Json.Serialization;

namespace Message.Presentation.Utilities;

public class UnixMillisToDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // 支持直接读取数字或字符串格式的时间戳
        return reader.TokenType switch
        {
            JsonTokenType.Number => DateTimeOffset.FromUnixTimeMilliseconds(reader.GetInt64()),
            JsonTokenType.String => DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(reader.GetString() ?? string.Empty)),
            _ => throw new JsonException("Expected number or string for millisecondsSinceEpoch.")
        };
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        // 序列化时转回毫秒时间戳
        writer.WriteNumberValue(value.ToUnixTimeMilliseconds());
    }
}