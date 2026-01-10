using System.Text.Json;
namespace PCIShield.BlazorMauiShared.Models;
public class DepthLimitedJsonConverter : System.Text.Json.Serialization.JsonConverter<object>
{
    private readonly int _maxDepth;
    public DepthLimitedJsonConverter(int maxDepth)
    {
        _maxDepth = maxDepth;
    }
    public override bool CanConvert(Type typeToConvert) => true;
    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize(ref reader, typeToConvert, options);
    }
    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        var newOptions = new JsonSerializerOptions(options);
        newOptions.Converters.Remove(this);
        if (writer.CurrentDepth >= _maxDepth)
        {
            if (value is System.Collections.IEnumerable enumerable)
            {
                writer.WriteStartArray();
                foreach (var item in enumerable)
                {
                    writer.WriteNullValue();
                }
                writer.WriteEndArray();
            }
            else
            {
                writer.WriteNullValue();
            }
        }
        else
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), newOptions);
        }
    }
}