using System.IO;
using System.Threading.Tasks;
using Utf8Json;
namespace PCIShield.Api.Common
{
    public static class Utf8JsonExtensions
    {
        public static byte[] SerializeToUtf8Bytes<T>(this T value) =>
            JsonSerializer.Serialize(value, Utf8JsonResolver.Instance);
        public static string SerializeToString<T>(this T value) =>
            JsonSerializer.ToJsonString(value, Utf8JsonResolver.Instance);
        public static T DeserializeFromUtf8Bytes<T>(this byte[] bytes) =>
            JsonSerializer.Deserialize<T>(bytes, Utf8JsonResolver.Instance);
        public static Task SerializeToStreamAsync<T>(this T value, Stream stream) =>
            JsonSerializer.SerializeAsync(stream, value, Utf8JsonResolver.Instance);
    }
}
