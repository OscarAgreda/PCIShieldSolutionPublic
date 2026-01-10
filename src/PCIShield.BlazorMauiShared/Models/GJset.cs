using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Utf8Json;
using Utf8Json.Formatters;
using Utf8Json.Resolvers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace PCIShield.BlazorMauiShared.Models
{
    public static class Utf8JsonSettings
    {

        private static readonly IJsonFormatterResolver Resolver = CreateResolver();
        public static byte[] Serialize<T>(T value) =>
            Utf8Json.JsonSerializer.Serialize(value, Resolver);
        public static T Deserialize<T>(byte[] bytes) =>
            Utf8Json.JsonSerializer.Deserialize<T>(bytes, Resolver);
        public static string SerializeToString<T>(T value) =>
            Utf8Json.JsonSerializer.ToJsonString(value, Resolver);
        public static T DeserializeFromString<T>(string json) =>
            Utf8Json.JsonSerializer.Deserialize<T>(json, Resolver);
        public static Task SerializeToStreamAsync<T>(T value, Stream stream) =>
            Utf8Json.JsonSerializer.SerializeAsync(stream, value, Resolver);
        private static IJsonFormatterResolver CreateResolver()
        {
            return CompositeResolver.Create(
                new IJsonFormatter[]
                {
                new DateTimeFormatter(),
                new NullableFormatter<DateTime>(),
                new JsonStringEnumFormatter(),
                new BooleanFormatter(),
                new DecimalFormatter()
                },
                new IJsonFormatterResolver[]
                {
                StandardResolver.AllowPrivate,
                StandardResolver.ExcludeNull,
                StandardResolver.CamelCase,
                }
            );
        }
        private class DateTimeFormatter : IJsonFormatter<DateTime>
        {
            public void Serialize(ref JsonWriter writer, DateTime value, IJsonFormatterResolver formatterResolver)
            {
                writer.WriteString(value.ToString("O"));
            }
            public DateTime Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
            {
                return DateTime.Parse(reader.ReadString());
            }
        }
        private class BooleanFormatter : IJsonFormatter<bool>
        {
            public void Serialize(ref JsonWriter writer, bool value, IJsonFormatterResolver formatterResolver)
            {
                writer.WriteBoolean(value);
            }
            public bool Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
            {
                try
                {
                    return reader.ReadBoolean();
                }
                catch
                {
                    var str = reader.ReadString();
                    return bool.Parse(str);
                }
            }
        }
        private class DecimalFormatter : IJsonFormatter<decimal>
        {
            public void Serialize(ref JsonWriter writer, decimal value, IJsonFormatterResolver formatterResolver)
            {
                writer.WriteString(value.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }
            public decimal Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
            {
                try
                {
                    return (decimal)reader.ReadDouble();
                }
                catch
                {
                    try
                    {
                        return decimal.Parse(
                            reader.ReadString(),
                            System.Globalization.CultureInfo.InvariantCulture
                        );
                    }
                    catch
                    {
                        var number = reader.ReadNumberSegment();
                        return decimal.Parse(
                            number.ToString(),
                            System.Globalization.CultureInfo.InvariantCulture
                        );
                    }
                }
            }
        }
        private class NullableFormatter<T> : IJsonFormatter<T?> where T : struct
        {
            public void Serialize(ref JsonWriter writer, T? value, IJsonFormatterResolver formatterResolver)
            {
                if (value == null)
                {
                    writer.WriteNull();
                    return;
                }
                formatterResolver.GetFormatterWithVerify<T>().Serialize(ref writer, value.Value, formatterResolver);
            }
            public T? Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
            {
                if (reader.ReadIsNull())
                    return null;
                return formatterResolver.GetFormatterWithVerify<T>().Deserialize(ref reader, formatterResolver);
            }
        }
        private class JsonStringEnumFormatter : IJsonFormatter<Enum>
        {
            public void Serialize(ref JsonWriter writer, Enum value, IJsonFormatterResolver formatterResolver)
            {
                if (value == null)
                {
                    writer.WriteNull();
                    return;
                }
                writer.WriteString(value.ToString());
            }
            public Enum Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
            {
                if (reader.ReadIsNull())
                    return null;
                throw new NotImplementedException("Generic enum deserialization not supported");
            }
        }
    }
    public class GJset
    {
        public static JsonSerializerOptions GetSystemTextJsonSettings()
        {
            return new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true,
                MaxDepth = 0,
                IgnoreReadOnlyFields = false,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                IgnoreReadOnlyProperties = false,
                PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace,
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }
        public static JsonSerializerOptions GetSystemTextJsonSettingsForList()
        {
            var options = GetSystemTextJsonSettings();
            options.PropertyNameCaseInsensitive = true;
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.WriteIndented = true;
            options.AllowTrailingCommas = true;
            options.ReadCommentHandling = JsonCommentHandling.Skip;
            options.NumberHandling = JsonNumberHandling.AllowReadingFromString;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.PreferredObjectCreationHandling = JsonObjectCreationHandling.Populate;
            options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.Converters.Add(new GenericDtoListConverter());
            return options;
        }

    }
}
