using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;

using PCIShieldLib.SharedKernel.Interfaces;
public class GenericDtoListConverter : System.Text.Json.Serialization.JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(IModelDto).IsAssignableFrom(typeToConvert);
    }
    public override System.Text.Json.Serialization.JsonConverter CreateConverter(Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
    {
        Type converterType = typeof(GenericDtoConverter<>).MakeGenericType(typeToConvert);
        return (System.Text.Json.Serialization.JsonConverter)Activator.CreateInstance(converterType, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null,
            null, null)!;
    }
    private class GenericDtoConverter<T> : System.Text.Json.Serialization.JsonConverter<T>
        where T : IModelDto
    {
        private const int MaxNestingLevel = 10;
        private const int MaxNestingLevelForCollections = 1;
        private static readonly HashSet<Type> SimpleTypes = new()
        {
            typeof(bool),
            typeof(byte),
            typeof(sbyte),
            typeof(char),
            typeof(decimal),
            typeof(double),
            typeof(float),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(short),
            typeof(ushort),
            typeof(string),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan),
            typeof(Guid),
            typeof(byte[])
        };
        public override T? Read(ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            Debug.WriteLine($"Starting to read {typeToConvert.Name}");
            if (reader.TokenType != System.Text.Json.JsonTokenType.StartObject)
            {
                Debug.WriteLine($"Expected StartObject but got {reader.TokenType}");
                throw new System.Text.Json.JsonException($"Expected StartObject but got {reader.TokenType}");
            }
            T? result = default;
            try
            {
                Debug.WriteLine("Checkpoint 1: Before calling ReadObject");
                result = (T?)ReadObject(typeToConvert, ref reader, options, 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in ReadObject: {ex}");
                throw;
            }
            int depth = 1;
            while (reader.Read())
            {
                Debug.WriteLine($"Consuming extra content: {reader.TokenType}");
                if (reader.TokenType == System.Text.Json.JsonTokenType.StartObject || reader.TokenType == System.Text.Json.JsonTokenType.StartArray)
                {
                    depth++;
                }
                else if (reader.TokenType == System.Text.Json.JsonTokenType.EndObject || reader.TokenType == System.Text.Json.JsonTokenType.EndArray)
                {
                    depth--;
                    if (depth == 0)
                    {
                        break;
                    }
                }
            }
            Debug.WriteLine($"Finished reading {typeToConvert.Name}");
            return result;
        }
        public override void Write(Utf8JsonWriter writer, T value, System.Text.Json.JsonSerializerOptions options)
        {
            WriteObject(writer, value, options, 0);
        }
        private bool InvoiceIdFound(System.Reflection.PropertyInfo property)
        {
            if (
                property.Name.Equals("InvoiceDate", StringComparison.OrdinalIgnoreCase) ||
            property.Name.Equals("InvoiceId", StringComparison.OrdinalIgnoreCase)
                )
            {
                Debug.WriteLine("Checkpoint: InvoiceId property found");
                return true;
            }
            return false;
        }
        private bool IsGenericEnumerable(Type type)
        {
            return type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>) ||
                                          type.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                                          type.GetGenericTypeDefinition() == typeof(ICollection<>));
        }
        private bool IsSimpleType(Type? type)
        {
            if (type == null)
            {
                return true;
            }
            if (SimpleTypes.Contains(type))
            {
                return true;
            }
            if (type.IsEnum)
            {
                return true;
            }
            Type? underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
            {
                return IsSimpleType(underlyingType);
            }
            return false;
        }
        private object? ReadObject(Type type, ref Utf8JsonReader reader, System.Text.Json.JsonSerializerOptions options,
            int nestingLevel)
        {
            Debug.WriteLine($"Checkpoint 2: Start of ReadObject for {type.Name}");
            if (reader.TokenType == System.Text.Json.JsonTokenType.Null)
            {
                Debug.WriteLine("ReadObject: Null value encountered");
                return null;
            }
            object? instance = Activator.CreateInstance(type);
            System.Reflection.PropertyInfo[] properties = type.GetProperties();
            Debug.WriteLine($"Checkpoint 3: Got {properties.Length} properties for {type.Name}");
            while (reader.Read())
            {
                Debug.WriteLine($"ReadObject: Token: {reader.TokenType}");
                if (reader.TokenType == System.Text.Json.JsonTokenType.EndObject)
                {
                    Debug.WriteLine("ReadObject: End of object reached");
                    return instance;
                }
                if (reader.TokenType != System.Text.Json.JsonTokenType.PropertyName)
                {
                    Debug.WriteLine($"ReadObject: Expected PropertyName but got {reader.TokenType}");
                    throw new System.Text.Json.JsonException($"Expected PropertyName but got {reader.TokenType}");
                }
                string propertyName = reader.GetString()!;
                Debug.WriteLine($"ReadObject: Property: {propertyName}");
                if (!reader.Read())
                {
                    Debug.WriteLine("ReadObject: Unexpected end when reading property value");
                    throw new System.Text.Json.JsonException("Unexpected end when reading property value");
                }
                Debug.WriteLine($"Checkpoint 4: Before property selection for {propertyName}");
                System.Reflection.PropertyInfo? property = properties.FirstOrDefault(p =>
                    options.PropertyNamingPolicy?.ConvertName(p.Name) == propertyName ||
                    p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
                Debug.WriteLine(
                    $"Checkpoint 5: Property selection result for {propertyName}: {(property != null ? property.Name : "null")}");
                if (property != null)
                {
                    object? value = null;
                    if (IsSimpleType(property.PropertyType))
                    {
                        Debug.WriteLine($"Checkpoint 6: Before reading simple type {property.Name}");
                        value = ReadValue(ref reader, property.PropertyType, options);
                        Debug.WriteLine($"ReadObject: Deserialized simple type {property.PropertyType.Name}: {value}");
                    }
                    else if (typeof(IModelDto).IsAssignableFrom(property.PropertyType) &&
                             nestingLevel < MaxNestingLevel)
                    {
                        value = ReadObject(property.PropertyType, ref reader, options, nestingLevel + 1);
                        Debug.WriteLine($"ReadObject: Deserialized nested DTO {property.PropertyType.Name}");
                    }
                    else if (IsGenericEnumerable(property.PropertyType) && nestingLevel < MaxNestingLevelForCollections)
                    {
                        System.Type elementType = property.PropertyType.GetGenericArguments()[0];
                        IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType))!;
                        if (reader.TokenType == System.Text.Json.JsonTokenType.StartArray)
                        {
                            Debug.WriteLine($"ReadObject: Starting to read array of {elementType.Name}");
                            while (reader.Read() && reader.TokenType != System.Text.Json.JsonTokenType.EndArray)
                            {
                                object? item = ReadObject(elementType, ref reader, options, nestingLevel + 1);
                                list.Add(item);
                            }
                            Debug.WriteLine($"ReadObject: Finished reading array of {elementType.Name}");
                        }
                        value = list;
                    }
                    else
                    {
                        Debug.WriteLine($"ReadObject: Skipping property {propertyName}");
                        reader.Skip();
                    }
                    property.SetValue(instance, value);
                }
                else
                {
                    Debug.WriteLine($"ReadObject: Property {propertyName} not found, skipping");
                    reader.Skip();
                }
            }
            Debug.WriteLine("ReadObject: Unexpected end when reading object");
            throw new System.Text.Json.JsonException("Unexpected end when reading object");
        }
        private static string ToCamelcase(string s)
        {
            if (s.Length > 0)
            {
                string input = Regex.Replace(s, "([A-Z]+|[0-9]+)", " $1", RegexOptions.Compiled).Trim();
                ;
                input = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input);
                var displayName = Regex.Replace(input, "([A-Z]+|[0-9]+)", " $1", RegexOptions.Compiled).Trim();
                input = displayName.Replace(" ", "");
                string output = char.ToLower(input[0]) + input.Substring(1);
                int amper = output.IndexOf("_", System.StringComparison.Ordinal);
                amper = output.IndexOf("_", amper + 1, System.StringComparison.Ordinal);
                if (amper > 0) output = output.ToLower();
                return output;
            }
            else
            {
                return s;
            }
        }
        private object? ReadValue(ref Utf8JsonReader reader, Type targetType, System.Text.Json.JsonSerializerOptions options)
        {
            Type underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;
            switch (reader.TokenType)
            {
                case System.Text.Json.JsonTokenType.True:
                    return Convert.ChangeType(true, underlyingType);
                case System.Text.Json.JsonTokenType.False:
                    return Convert.ChangeType(false, underlyingType);
                case System.Text.Json.JsonTokenType.Number:
                    if (underlyingType == typeof(int))
                    {
                        return reader.GetInt32();
                    }
                    if (underlyingType == typeof(long))
                    {
                        return reader.GetInt64();
                    }
                    if (underlyingType == typeof(float))
                    {
                        return reader.GetSingle();
                    }
                    if (underlyingType == typeof(double))
                    {
                        return reader.GetDouble();
                    }
                    if (underlyingType == typeof(decimal))
                    {
                        return reader.GetDecimal();
                    }
                    if (underlyingType == typeof(byte))
                    {
                        return Convert.ToByte(reader.GetInt32());
                    }
                    if (underlyingType == typeof(sbyte))
                    {
                        return Convert.ToSByte(reader.GetInt32());
                    }
                    if (underlyingType == typeof(short))
                    {
                        return Convert.ToInt16(reader.GetInt32());
                    }
                    if (underlyingType == typeof(ushort))
                    {
                        return Convert.ToUInt16(reader.GetInt32());
                    }
                    if (underlyingType == typeof(uint))
                    {
                        return Convert.ToUInt32(reader.GetInt64());
                    }
                    if (underlyingType == typeof(ulong))
                    {
                        return Convert.ToUInt64(reader.GetInt64());
                    }
                    break;
                case JsonTokenType.String:
                    string? stringValue = reader.GetString();
                    if (underlyingType == typeof(string))
                    {
                        return stringValue;
                    }
                    if (underlyingType == typeof(char) && stringValue?.Length == 1)
                    {
                        return stringValue[0];
                    }
                    if (underlyingType == typeof(Guid) && Guid.TryParse(stringValue, out Guid guid))
                    {
                        return guid;
                    }
                    if (underlyingType == typeof(DateTime) && DateTime.TryParse(stringValue, out DateTime dateTime))
                    {
                        return dateTime;
                    }
                    if (underlyingType == typeof(DateTimeOffset) &&
                        DateTimeOffset.TryParse(stringValue, out DateTimeOffset dateTimeOffset))
                    {
                        return dateTimeOffset;
                    }
                    if (underlyingType == typeof(TimeSpan) && TimeSpan.TryParse(stringValue, out TimeSpan timeSpan))
                    {
                        return timeSpan;
                    }
                    if (underlyingType.IsEnum)
                    {
                        return Enum.Parse(underlyingType, stringValue!, true);
                    }
                    break;
                case System.Text.Json.JsonTokenType.StartArray:
                    if (underlyingType == typeof(byte[]))
                    {
                        return JsonSerializer.Deserialize<byte[]>(ref reader, options);
                    }
                    break;
                case System.Text.Json.JsonTokenType.Null:
                    if (targetType.IsValueType && Nullable.GetUnderlyingType(targetType) == null)
                    {
                        throw new System.Text.Json.JsonException($"Cannot assign null to non-nullable type {targetType}.");
                    }
                    return null;
            }
            return System.Text.Json.JsonSerializer.Deserialize(ref reader, targetType, options);
        }
        private bool ShouldExcludeProperty(System.Reflection.PropertyInfo property)
        {
            return property.GetCustomAttributes(typeof(System.Text.Json.Serialization.JsonIgnoreAttribute), true).Any();
        }
        private void WriteObject(Utf8JsonWriter writer, object value, System.Text.Json.JsonSerializerOptions options, int nestingLevel)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }
            writer.WriteStartObject();
            Type type = value.GetType();
            System.Reflection.PropertyInfo[] properties = type.GetProperties();
            foreach (System.Reflection.PropertyInfo property in properties)
            {
                object? propertyValue = property.GetValue(value);
                if (!ShouldExcludeProperty(property) && propertyValue != null)
                {
                    writer.WritePropertyName(options.PropertyNamingPolicy?.ConvertName(property.Name) ?? property.Name);
                    if (IsSimpleType(property.PropertyType))
                    {
                        JsonSerializer.Serialize(writer, propertyValue, property.PropertyType, options);
                    }
                    else if (typeof(IModelDto).IsAssignableFrom(property.PropertyType))
                    {
                        if (nestingLevel < MaxNestingLevel)
                        {
                            WriteObject(writer, propertyValue, options, nestingLevel + 1);
                        }
                        else
                        {
                            WritePrimitiveProperties(writer, propertyValue, options);
                        }
                    }
                    else if (IsGenericEnumerable(property.PropertyType))
                    {
                        writer.WriteStartArray();
                        IEnumerable list = (IEnumerable)propertyValue;
                        foreach (object? item in list)
                        {
                            if (nestingLevel < MaxNestingLevel)
                            {
                                WriteObject(writer, item, options, nestingLevel + 1);
                            }
                            else if (IsSimpleType(item.GetType()))
                            {
                                JsonSerializer.Serialize(writer, item, item.GetType(), options);
                            }
                            else
                            {
                                WritePrimitiveProperties(writer, item, options);
                            }
                        }
                        writer.WriteEndArray();
                    }
                }
            }
            writer.WriteEndObject();
        }
        private void WritePrimitiveProperties(Utf8JsonWriter writer, object value, System.Text.Json.JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            foreach (System.Reflection.PropertyInfo prop in value.GetType().GetProperties())
            {
                if (IsSimpleType(prop.PropertyType))
                {
                    object? propValue = prop.GetValue(value);
                    if (propValue != null)
                    {
                        writer.WritePropertyName(options.PropertyNamingPolicy?.ConvertName(prop.Name) ?? prop.Name);
                        System.Text.Json.JsonSerializer.Serialize(writer, propValue, prop.PropertyType, options);
                    }
                }
            }
            writer.WriteEndObject();
        }
    }
}