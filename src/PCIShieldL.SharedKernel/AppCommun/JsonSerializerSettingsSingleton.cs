using System.Text.Json;
using System.Text.Json.Serialization;
namespace PCIShieldLib.SharedKernel
{
    public sealed class JsonSerializerSettingsSingleton
    {
        private JsonSerializerSettingsSingleton()
        { }
        public static JsonSerializerOptions Instance { get; } = new()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            ReferenceHandler = ReferenceHandler.Preserve,
            WriteIndented = true,
            MaxDepth = 0,
            IgnoreReadOnlyFields = false,
        };
    }
}