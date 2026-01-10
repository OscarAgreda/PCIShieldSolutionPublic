using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using LanguageExt;
using Microsoft.CodeAnalysis.Options;
namespace PCIShield.Infrastructure.Services
{
    public interface IJsonDifferenceFinderService
    {
        LanguageExt.Option<JsonDocument> FindDifferences(JsonElement oldObj, JsonElement newObj);
    }
    public enum DifferenceStatus
    {
        Added,
        Removed,
        Changed
    }
    public class JsonDifferenceFinderService : IJsonDifferenceFinderService
    {
        private readonly IAppLoggerService<JsonDifferenceFinderService> _logger;
        public JsonDifferenceFinderService(IAppLoggerService<JsonDifferenceFinderService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public LanguageExt.Option<JsonDocument> FindDifferences(
            JsonElement oldObj,
            JsonElement newObj
        )
        {
            if (
                oldObj.ValueKind == JsonValueKind.Null
                || oldObj.ValueKind == JsonValueKind.Undefined
                || newObj.ValueKind == JsonValueKind.Null
                || newObj.ValueKind == JsonValueKind.Undefined
            )
            {
                _logger.LogError("Either oldObj or newObj is null or undefined.");
                return LanguageExt.Option<JsonDocument>.None;
            }
            var jsonDifferenceBuilder = new JsonDifferenceBuilder(estimatedSize: 50);
            CompareJsonElements(oldObj, newObj, string.Empty, jsonDifferenceBuilder);
            var differencesDoc = jsonDifferenceBuilder.Build();
            return differencesDoc.RootElement.ValueKind != JsonValueKind.Null
                ? LanguageExt.Option<JsonDocument>.Some(differencesDoc)
                : LanguageExt.Option<JsonDocument>.None;
        }
        private static void CompareArrayElements(
            JsonElement oldElement,
            JsonElement newElement,
            string path,
            JsonDifferenceBuilder builder
        )
        {
            var oldArray = oldElement.EnumerateArray().ToArray();
            var newArray = newElement.EnumerateArray().ToArray();
            if (!oldArray.Any() && !newArray.Any())
            {
                return;
            }
            for (int i = 0; i < oldArray.Length; i++)
            {
                if (i >= newArray.Length)
                {
                    builder.Add(
                        $"{path}[{i}]",
                        new
                        {
                            Status = DifferenceStatus.Removed,
                            Value = ConvertJsonElementToObject(oldArray[i])
                        }
                    );
                }
            }
            for (int i = 0; i < newArray.Length; i++)
            {
                if (i >= oldArray.Length)
                {
                    builder.Add(
                        $"{path}[{i}]",
                        new
                        {
                            Status = DifferenceStatus.Added,
                            Value = ConvertJsonElementToObject(newArray[i])
                        }
                    );
                    continue;
                }
                var oldVal = oldArray[i];
                var newVal = newArray[i];
                if (
                    oldVal.ValueKind == JsonValueKind.Object
                    && newVal.ValueKind == JsonValueKind.Object
                )
                {
                    CompareJsonElements(oldVal, newVal, $"{path}[{i}]", builder);
                }
                else if (!oldVal.Equals(newVal))
                {
                    builder.Add(
                        $"{path}[{i}]",
                        new
                        {
                            Status = DifferenceStatus.Changed,
                            OldValue = ConvertJsonElementToObject(oldVal),
                            NewValue = ConvertJsonElementToObject(newVal)
                        }
                    );
                }
            }
        }
        private static void CompareJsonElements(
            JsonElement oldElement,
            JsonElement newElement,
            string path,
            JsonDifferenceBuilder builder
        )
        {
            if (oldElement.ValueKind != newElement.ValueKind)
            {
                builder.Add(
                    path,
                    new
                    {
                        Status = DifferenceStatus.Changed,
                        OldValue = ConvertJsonElementToObject(oldElement),
                        NewValue = ConvertJsonElementToObject(newElement)
                    }
                );
                return;
            }
            if (oldElement.ValueKind == JsonValueKind.Object)
            {
                CompareObjectElements(oldElement, newElement, path, builder);
            }
            else if (oldElement.ValueKind == JsonValueKind.Array)
            {
                CompareArrayElements(oldElement, newElement, path, builder);
            }
            else
            {
                if (!oldElement.Equals(newElement))
                {
                    builder.Add(
                        path,
                        new
                        {
                            Status = DifferenceStatus.Changed,
                            OldValue = ConvertJsonElementToObject(oldElement),
                            NewValue = ConvertJsonElementToObject(newElement)
                        }
                    );
                }
            }
        }
        private static void CompareObjectElements(
            JsonElement oldElement,
            JsonElement newElement,
            string path,
            JsonDifferenceBuilder builder
        )
        {
            foreach (var property in oldElement.EnumerateObject())
            {
                var propertyName = property.Name;
                var propertyPath = string.IsNullOrEmpty(path)
                    ? propertyName
                    : $"{path}.{propertyName}";
                if (newElement.TryGetProperty(propertyName, out var newVal))
                {
                    CompareJsonElements(property.Value, newVal, propertyPath, builder);
                }
                else
                {
                    builder.Add(
                        propertyPath,
                        new
                        {
                            Status = DifferenceStatus.Removed,
                            Value = ConvertJsonElementToObject(property.Value)
                        }
                    );
                }
            }
            foreach (var property in newElement.EnumerateObject())
            {
                var propertyName = property.Name;
                var propertyPath = string.IsNullOrEmpty(path)
                    ? propertyName
                    : $"{path}.{propertyName}";
                if (!oldElement.TryGetProperty(propertyName, out _))
                {
                    builder.Add(
                        propertyPath,
                        new
                        {
                            Status = DifferenceStatus.Added,
                            Value = ConvertJsonElementToObject(property.Value)
                        }
                    );
                }
            }
        }
        private static object ConvertJsonElementToObject(JsonElement value)
        {
            return value.ValueKind switch
            {
                JsonValueKind.Object
                    => JsonSerializer.Deserialize<Dictionary<string, object>>(value.GetRawText()),
                JsonValueKind.Array => JsonSerializer.Deserialize<List<object>>(value.GetRawText()),
                JsonValueKind.String => value.GetString(),
                JsonValueKind.Number => value.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null,
                _ => null
            };
        }
    }
    public class JsonDifferenceBuilder
    {
        private readonly Dictionary<string, object> _data;
        public JsonDifferenceBuilder(int? estimatedSize = null)
        {
            _data = estimatedSize.HasValue
                ? new Dictionary<string, object>(estimatedSize.Value)
                : new Dictionary<string, object>();
        }
        public void Add(string key, object value)
        {
            _data[key] = value;
        }
        public JsonDocument Build()
        {
            string json = JsonSerializer.Serialize(_data);
            return JsonDocument.Parse(json);
        }
    }
}
