using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
namespace Shared.Toon
{
    public static class ToonUtility
    {
        public sealed class ToonEncodingOptions
        {
            public static ToonEncodingOptions Default { get; } = new ToonEncodingOptions();
            public int IndentSize { get; init; } = 2;
            public char Delimiter { get; init; } = ',';
        }

        public sealed class ToonDecodeException : Exception
        {
            public int? Line { get; }
            public int? Column { get; }

            internal ToonDecodeException(string message, int? line, int? column, Exception inner)
                : base(message, inner)
            {
                Line = line;
                Column = column;
            }
        }

        private static readonly JsonSerializerOptions DefaultJsonSerializerOptions =
            BuildDefaultJsonOptions();
        public static JsonSerializerOptions DefaultJsonOptions => DefaultJsonSerializerOptions;

        public static JsonSerializerOptions CreateDefaultJsonOptions() =>
            new JsonSerializerOptions(DefaultJsonSerializerOptions);

        public static string ToToon<T>(
            T value,
            JsonSerializerOptions? jsonOptions = null,
            ToonEncodingOptions? toonOptions = null
        )
        {
            jsonOptions ??= DefaultJsonSerializerOptions;
            toonOptions ??= ToonEncodingOptions.Default;
            JsonElement root = System.Text.Json.JsonSerializer.SerializeToElement(
                value!,
                jsonOptions
            );
            var sb = new StringBuilder();
            EncodeElement(root, key: null, depth: 0, sb, toonOptions, toonOptions.Delimiter);
            return sb.ToString();
        }

        public static string ToToon(JsonElement element, ToonEncodingOptions? toonOptions = null)
        {
            toonOptions ??= ToonEncodingOptions.Default;
            var sb = new StringBuilder();
            EncodeElement(element, key: null, depth: 0, sb, toonOptions, toonOptions.Delimiter);
            return sb.ToString();
        }

        public static string JsonToToon(
            string json,
            JsonSerializerOptions? jsonOptions = null,
            ToonEncodingOptions? toonOptions = null
        )
        {
            if (json is null)
                throw new ArgumentNullException(nameof(json));
            jsonOptions ??= DefaultJsonSerializerOptions;
            toonOptions ??= ToonEncodingOptions.Default;
            using var doc = JsonDocument.Parse(
                json,
                new JsonDocumentOptions
                {
                    AllowTrailingCommas = jsonOptions.AllowTrailingCommas,
                    CommentHandling = jsonOptions.ReadCommentHandling,
                }
            );
            return ToToon(doc.RootElement, toonOptions);
        }

        public static void ToToon<T>(
            T value,
            Stream stream,
            JsonSerializerOptions? jsonOptions = null,
            ToonEncodingOptions? toonOptions = null,
            Encoding? encoding = null
        )
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));
            string toon = ToToon(value, jsonOptions, toonOptions);
            encoding ??= Encoding.UTF8;
            using var writer = new StreamWriter(stream, encoding, leaveOpen: true);
            writer.Write(toon);
            writer.Flush();
        }

        public static async Task ToToonAsync<T>(
            T value,
            Stream stream,
            JsonSerializerOptions? jsonOptions = null,
            ToonEncodingOptions? toonOptions = null,
            Encoding? encoding = null,
            CancellationToken cancellationToken = default
        )
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));
            string toon = ToToon(value, jsonOptions, toonOptions);
            encoding ??= Encoding.UTF8;
            byte[] buffer = encoding.GetBytes(toon);
#if NETSTANDARD2_0
            await stream
                .WriteAsync(buffer, 0, buffer.Length, cancellationToken)
                .ConfigureAwait(false);
#else
            await stream
                .WriteAsync(buffer.AsMemory(0, buffer.Length), cancellationToken)
                .ConfigureAwait(false);
#endif
        }

        public static JsonElement ParseToJsonElement(
            string toon,
            JsonSerializerOptions? jsonOptions = null
        )
        {
            if (toon is null)
                throw new ArgumentNullException(nameof(toon));
            jsonOptions ??= DefaultJsonSerializerOptions;
            try
            {
                var parser = new InternalToonParser();
                object? model = parser.Parse(toon);
                return System.Text.Json.JsonSerializer.SerializeToElement<object?>(
                    model,
                    jsonOptions
                );
            }
            catch (InternalToonSyntaxError ex)
            {
                throw new ToonDecodeException(ex.Message, ex.Line, ex.Column, ex);
            }
        }

        public static string ToonToJson(string toon, JsonSerializerOptions? jsonOptions = null)
        {
            if (toon is null)
                throw new ArgumentNullException(nameof(toon));
            jsonOptions ??= DefaultJsonSerializerOptions;
            try
            {
                var parser = new InternalToonParser();
                object? model = parser.Parse(toon);
                return System.Text.Json.JsonSerializer.Serialize<object?>(model, jsonOptions);
            }
            catch (InternalToonSyntaxError ex)
            {
                throw new ToonDecodeException(ex.Message, ex.Line, ex.Column, ex);
            }
        }

        public static T FromToon<T>(string toon, JsonSerializerOptions? jsonOptions = null)
        {
            if (toon is null)
                throw new ArgumentNullException(nameof(toon));
            jsonOptions ??= DefaultJsonSerializerOptions;
            try
            {
                var parser = new InternalToonParser();
                object? model = parser.Parse(toon);
                if (typeof(T) == typeof(object))
                {
                    return (T)model!;
                }
                if (typeof(T) == typeof(JsonElement))
                {
                    var element = System.Text.Json.JsonSerializer.SerializeToElement<object?>(
                        model,
                        jsonOptions
                    );
                    return (T)(object)element;
                }
                string json = System.Text.Json.JsonSerializer.Serialize<object?>(
                    model,
                    jsonOptions
                );
                T? result = System.Text.Json.JsonSerializer.Deserialize<T>(json, jsonOptions);
                if (result is null)
                {
                    throw new ToonDecodeException(
                        $"TOON content could not be deserialized to type {typeof(T).FullName}.",
                        line: null,
                        column: null,
                        inner: null!
                    );
                }
                return result;
            }
            catch (InternalToonSyntaxError ex)
            {
                throw new ToonDecodeException(ex.Message, ex.Line, ex.Column, ex);
            }
        }

        public static async Task<T> FromToonAsync<T>(
            Stream stream,
            JsonSerializerOptions? jsonOptions = null,
            Encoding? encoding = null,
            CancellationToken cancellationToken = default
        )
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));
            encoding ??= Encoding.UTF8;
            using var reader = new StreamReader(
                stream,
                encoding,
                detectEncodingFromByteOrderMarks: true,
                leaveOpen: true
            );
#if NETSTANDARD2_0
            string toon = await reader.ReadToEndAsync().ConfigureAwait(false);
#else
            string toon = await reader.ReadToEndAsync().ConfigureAwait(false);
#endif
            return FromToon<T>(toon, jsonOptions);
        }

        private static JsonSerializerOptions BuildDefaultJsonOptions()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = false,
                MaxDepth = 0,
                IgnoreReadOnlyFields = false,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                IgnoreReadOnlyProperties = false,
                PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace,
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            return options;
        }

        private static void EncodeElement(
            JsonElement element,
            string? key,
            int depth,
            StringBuilder sb,
            ToonEncodingOptions options,
            char activeDelimiter
        )
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    EncodeObject(element, key, depth, sb, options);
                    break;
                case JsonValueKind.Array:
                    EncodeArray(element, key, depth, sb, options);
                    break;
                case JsonValueKind.String:
                case JsonValueKind.Number:
                case JsonValueKind.True:
                case JsonValueKind.False:
                case JsonValueKind.Null:
                    EncodePrimitiveElement(element, key, depth, sb, options, activeDelimiter);
                    break;
                default:
                    throw new NotSupportedException(
                        $"Unsupported JSON value kind for TOON encoding: {element.ValueKind}"
                    );
            }
        }

        private static void EncodeObject(
            JsonElement obj,
            string? key,
            int depth,
            StringBuilder sb,
            ToonEncodingOptions options
        )
        {
            var properties = obj.EnumerateObject().ToList();
            string indent = Indent(depth, options);
            if (key is null)
            {
                foreach (var prop in properties)
                {
                    EncodeElement(
                        prop.Value,
                        key: prop.Name,
                        depth: depth,
                        sb,
                        options,
                        options.Delimiter
                    );
                }
                return;
            }
            string encodedKey = EncodeKey(key, options);
            if (properties.Count == 0)
            {
                sb.Append(indent).Append(encodedKey).Append(':').AppendLine();
                return;
            }
            sb.Append(indent).Append(encodedKey).Append(':').AppendLine();
            foreach (var prop in properties)
            {
                EncodeElement(
                    prop.Value,
                    key: prop.Name,
                    depth: depth + 1,
                    sb,
                    options,
                    options.Delimiter
                );
            }
        }

        private static void EncodeArray(
            JsonElement array,
            string? key,
            int depth,
            StringBuilder sb,
            ToonEncodingOptions options
        )
        {
            int length = array.GetArrayLength();
            string indent = Indent(depth, options);
            char delimiter = options.Delimiter;
            if (length == 0)
            {
                string header = FormatArrayHeader(key, length, delimiter, fields: null, options);
                sb.Append(indent).Append(header).AppendLine();
                return;
            }
            if (IsPrimitiveArray(array))
            {
                EncodePrimitiveArray(array, key, depth, sb, options);
                return;
            }
            if (TryGetTabularSchema(array, out var fields))
            {
                EncodeTabularArray(array, key, fields, depth, sb, options);
                return;
            }
            EncodeListArray(array, key, depth, sb, options);
        }

        private static void EncodePrimitiveArray(
            JsonElement array,
            string? key,
            int depth,
            StringBuilder sb,
            ToonEncodingOptions options
        )
        {
            int length = array.GetArrayLength();
            string indent = Indent(depth, options);
            char delimiter = options.Delimiter;
            string header = FormatArrayHeader(key, length, delimiter, fields: null, options);
            var values = new List<string>(length);
            foreach (var item in array.EnumerateArray())
            {
                values.Add(EncodePrimitive(item, delimiter));
            }
            sb.Append(indent)
                .Append(header)
                .Append(' ')
                .Append(string.Join(delimiter, values))
                .AppendLine();
        }

        private static void EncodeTabularArray(
            JsonElement array,
            string? key,
            string[] fields,
            int depth,
            StringBuilder sb,
            ToonEncodingOptions options
        )
        {
            int length = array.GetArrayLength();
            string indent = Indent(depth, options);
            string rowIndent = Indent(depth + 1, options);
            char delimiter = options.Delimiter;
            string encodedFields = string.Join(
                delimiter,
                fields.Select(f => EncodeKey(f, options))
            );
            string header = FormatArrayHeader(key, length, delimiter, encodedFields, options);
            sb.Append(indent).Append(header).AppendLine();
            foreach (var obj in array.EnumerateArray())
            {
                var dict = obj.EnumerateObject().ToDictionary(p => p.Name, p => p.Value);
                var rowValues = new List<string>(fields.Length);
                foreach (string field in fields)
                {
                    rowValues.Add(EncodePrimitive(dict[field], delimiter));
                }
                sb.Append(rowIndent).Append(string.Join(delimiter, rowValues)).AppendLine();
            }
        }

        private static void EncodeListArray(
            JsonElement array,
            string? key,
            int depth,
            StringBuilder sb,
            ToonEncodingOptions options
        )
        {
            int length = array.GetArrayLength();
            string indent = Indent(depth, options);
            string itemIndent = Indent(depth + 1, options);
            char delimiter = options.Delimiter;
            string header = FormatArrayHeader(key, length, delimiter, fields: null, options);
            sb.Append(indent).Append(header).AppendLine();
            foreach (var item in array.EnumerateArray())
            {
                if (IsPrimitive(item))
                {
                    sb.Append(itemIndent)
                        .Append("- ")
                        .Append(EncodePrimitive(item, delimiter))
                        .AppendLine();
                }
                else
                {
                    sb.Append(itemIndent).Append("-").AppendLine();
                    EncodeElement(item, key: null, depth: depth + 2, sb, options, delimiter);
                }
            }
        }

        private static void EncodePrimitiveElement(
            JsonElement element,
            string? key,
            int depth,
            StringBuilder sb,
            ToonEncodingOptions options,
            char activeDelimiter
        )
        {
            string indent = Indent(depth, options);
            string value = EncodePrimitive(element, activeDelimiter);
            if (key is null)
            {
                sb.Append(indent).Append(value).AppendLine();
                return;
            }
            string encodedKey = EncodeKey(key, options);
            sb.Append(indent).Append(encodedKey).Append(": ").Append(value).AppendLine();
        }

        private static string EncodePrimitive(JsonElement element, char activeDelimiter)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.String:
                    return EncodeString(element.GetString() ?? string.Empty, activeDelimiter);
                case JsonValueKind.Number:
                case JsonValueKind.True:
                case JsonValueKind.False:
                case JsonValueKind.Null:
                    return element.GetRawText();
                default:
                    throw new NotSupportedException(
                        $"JSON value kind {element.ValueKind} is not a primitive in TOON."
                    );
            }
        }

        private static bool IsPrimitive(JsonElement element)
        {
            return element.ValueKind == JsonValueKind.String
                || element.ValueKind == JsonValueKind.Number
                || element.ValueKind == JsonValueKind.True
                || element.ValueKind == JsonValueKind.False
                || element.ValueKind == JsonValueKind.Null;
        }

        private static bool IsPrimitiveArray(JsonElement array)
        {
            foreach (var item in array.EnumerateArray())
            {
                if (!IsPrimitive(item))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool TryGetTabularSchema(JsonElement array, out string[] fields)
        {
            fields = Array.Empty<string>();
            int length = array.GetArrayLength();
            if (length == 0)
            {
                return false;
            }
            var elements = array.EnumerateArray().ToArray();
            if (elements.Any(e => e.ValueKind != JsonValueKind.Object))
            {
                return false;
            }
            var firstProps = elements[0].EnumerateObject().ToArray();
            if (firstProps.Length == 0)
            {
                return false;
            }
            fields = firstProps.Select(p => p.Name).ToArray();
            foreach (var element in elements)
            {
                var props = element.EnumerateObject().ToArray();
                if (props.Length != fields.Length)
                {
                    return false;
                }
                var map = props.ToDictionary(p => p.Name, p => p.Value);
                foreach (string field in fields)
                {
                    if (!map.TryGetValue(field, out var value))
                    {
                        return false;
                    }
                    if (!IsPrimitive(value))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static string EncodeKey(string key, ToonEncodingOptions options)
        {
            return EncodeString(key, options.Delimiter);
        }

        private static string FormatArrayHeader(
            string? key,
            int length,
            char delimiter,
            string? fields,
            ToonEncodingOptions options
        )
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(key))
            {
                sb.Append(EncodeKey(key!, options));
            }
            sb.Append('[').Append(length);
            if (delimiter == '\t')
            {
                sb.Append('\t');
            }
            else if (delimiter == '|')
            {
                sb.Append('|');
            }
            sb.Append(']');
            if (!string.IsNullOrEmpty(fields))
            {
                sb.Append('{').Append(fields).Append('}');
            }
            sb.Append(':');
            return sb.ToString();
        }

        private static string EncodeString(string value, char activeDelimiter)
        {
            if (!RequiresQuotes(value, activeDelimiter))
            {
                return value;
            }
            return "\"" + EscapeString(value) + "\"";
        }

        private static bool RequiresQuotes(string value, char activeDelimiter)
        {
            if (value.Length == 0)
            {
                return true;
            }
            if (char.IsWhiteSpace(value[0]) || char.IsWhiteSpace(value[^1]))
            {
                return true;
            }
            if (value is "true" or "false" or "null")
            {
                return true;
            }
            if (LooksLikeNumber(value))
            {
                return true;
            }
            if (
                value.IndexOfAny(new[] { ':', '"', '\\', '[', ']', '{', '}', '\n', '\r', '\t' })
                >= 0
            )
            {
                return true;
            }
            if (value.IndexOf(activeDelimiter) >= 0)
            {
                return true;
            }
            if (value == "-" || value.StartsWith("-", StringComparison.Ordinal))
            {
                return true;
            }
            return false;
        }

        private static bool LooksLikeNumber(string value)
        {
            return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out _);
        }

        private static string EscapeString(string value)
        {
            var sb = new StringBuilder(value.Length + 8);
            foreach (char c in value)
            {
                switch (c)
                {
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '"':
                        sb.Append("\\\"");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }

        private static string Indent(int depth, ToonEncodingOptions options)
        {
            int spaces = depth * options.IndentSize;
            return spaces == 0 ? string.Empty : new string(' ', spaces);
        }

        private abstract class InternalToonError : Exception
        {
            protected InternalToonError(string message, Exception? inner = null)
                : base(message, inner) { }
        }

        private sealed class InternalToonSyntaxError : InternalToonError
        {
            public int? Line { get; }
            public int? Column { get; }

            public InternalToonSyntaxError(string message, int? line = null, int? column = null)
                : base(FormatMessage(message, line, column))
            {
                Line = line;
                Column = column;
            }

            private static string FormatMessage(string message, int? line, int? column)
            {
                if (line is null)
                {
                    return message;
                }
                if (column is null)
                {
                    return $"(line {line}) {message}";
                }
                return $"(line {line}, column {column}) {message}";
            }
        }

        private sealed class Line
        {
            public int Indent { get; }
            public string Content { get; }
            public int LineNo { get; }

            public Line(int indent, string content, int lineNo)
            {
                Indent = indent;
                Content = content;
                LineNo = lineNo;
            }
        }

        private sealed class InternalToonLexer
        {
            private readonly string _source;

            public InternalToonLexer(string source)
            {
                _source = source.Replace("\r\n", "\n").Replace("\r", "\n");
            }

            public List<Line> IterLines()
            {
                var text = RemoveBlockComments(_source);
                var lines = new List<Line>();
                var rawLines = text.Split('\n');
                for (int idx = 0; idx < rawLines.Length; idx++)
                {
                    var raw = rawLines[idx];
                    var stripped = StripInlineComment(raw);
                    if (string.IsNullOrWhiteSpace(stripped))
                    {
                        continue;
                    }
                    var span = stripped.AsSpan();
                    int leading = 0;
                    while (leading < span.Length && (span[leading] == ' ' || span[leading] == '\t'))
                    {
                        if (span[leading] == '\t')
                        {
                            throw new InternalToonSyntaxError(
                                "Tabs are not allowed for indentation",
                                line: idx + 1,
                                column: leading + 1
                            );
                        }
                        leading++;
                    }
                    int indent = leading;
                    string content = span.Slice(leading).TrimEnd().ToString();
                    lines.Add(new Line(indent, content, idx + 1));
                }
                return lines;
            }

            private static string RemoveBlockComments(string text)
            {
                var result = new StringBuilder();
                int depth = 0;
                int i = 0;
                while (i < text.Length)
                {
                    if (i < text.Length - 1 && text.AsSpan(i, 2).SequenceEqual("/*".AsSpan()))
                    {
                        depth++;
                        i += 2;
                        continue;
                    }
                    if (depth > 0)
                    {
                        if (i < text.Length - 1 && text.AsSpan(i, 2).SequenceEqual("*/".AsSpan()))
                        {
                            depth--;
                            i += 2;
                            continue;
                        }
                        result.Append(text[i] == '\n' ? '\n' : ' ');
                        i++;
                        continue;
                    }
                    result.Append(text[i]);
                    i++;
                }
                if (depth != 0)
                {
                    throw new InternalToonSyntaxError("Unterminated block comment");
                }
                return result.ToString();
            }

            private static string StripInlineComment(string line)
            {
                var buf = new StringBuilder();
                bool inString = false;
                bool escape = false;
                for (int i = 0; i < line.Length; i++)
                {
                    char ch = line[i];
                    if (escape)
                    {
                        buf.Append(ch);
                        escape = false;
                        continue;
                    }
                    if (ch == '\\')
                    {
                        buf.Append(ch);
                        escape = true;
                        continue;
                    }
                    if (ch == '"')
                    {
                        buf.Append(ch);
                        inString = !inString;
                        continue;
                    }
                    if (!inString)
                    {
                        if (ch == '#')
                        {
                            break;
                        }
                        if (i < line.Length - 1 && line.AsSpan(i, 2).SequenceEqual("//".AsSpan()))
                        {
                            break;
                        }
                    }
                    buf.Append(ch);
                }
                return buf.ToString().TrimEnd();
            }
        }

        private sealed class InternalToonParser
        {
            private readonly string _mode;
            private List<Line> _lines = new();
            private int _currentIndex;
            private static readonly Regex FoldableSegmentRegex = new(
                @"^[A-Za-z_][A-Za-z0-9_]*$",
                RegexOptions.Compiled
            );
            private const string DefaultTableDelimiter = ",";

            public InternalToonParser(string mode = "strict")
            {
                _mode = string.IsNullOrWhiteSpace(mode) ? "strict" : mode;
            }

            public object? Parse(string source)
            {
                var lexer = new InternalToonLexer(source);
                _lines = lexer.IterLines();
                _currentIndex = 0;
                if (_lines.Count == 0)
                {
                    return null;
                }
                return ParseValue(expectedIndent: 0);
            }

            private object? ParseValue(int expectedIndent)
            {
                if (_currentIndex >= _lines.Count)
                {
                    return null;
                }
                var line = _lines[_currentIndex];
                if (line.Indent < expectedIndent)
                {
                    return null;
                }
                if (line.Indent > expectedIndent)
                {
                    return null;
                }
                if (line.Content.StartsWith("-", StringComparison.Ordinal))
                {
                    return ParseArray(expectedIndent);
                }
                if (line.Content.Contains(":", StringComparison.Ordinal))
                {
                    return ParseObject(expectedIndent);
                }
                if (_mode == "strict" && LooksLikeMissingColon(line.Content))
                {
                    throw new InternalToonSyntaxError(
                        $"Expected ':' after key-like token: {line.Content.Trim()}",
                        line.LineNo
                    );
                }
                var scalar = ParseScalar(line.Content);
                _currentIndex++;
                return scalar;
            }

            private Dictionary<string, object?> ParseObject(int expectedIndent)
            {
                var result = new Dictionary<string, object?>();
                int baseIndent = expectedIndent;
                while (_currentIndex < _lines.Count)
                {
                    var line = _lines[_currentIndex];
                    if (line.Indent < baseIndent)
                    {
                        break;
                    }
                    if (line.Indent != baseIndent)
                    {
                        _currentIndex++;
                        continue;
                    }
                    var tableHeader = ParseTableHeader(line.Content);
                    if (tableHeader != null)
                    {
                        var (rows, nextIndex) = ParseTableFromHeader(
                            start: _currentIndex,
                            fields: tableHeader.Fields,
                            expectedLength: tableHeader.Count,
                            delimiter: tableHeader.Delimiter,
                            indent: baseIndent
                        );
                        AssignValue(
                            result,
                            tableHeader.Key,
                            rows,
                            tableHeader.AllowPathExpansion,
                            line.LineNo
                        );
                        _currentIndex = nextIndex;
                        continue;
                    }
                    if (!line.Content.Contains(":", StringComparison.Ordinal))
                    {
                        if (_mode == "strict")
                        {
                            throw new InternalToonSyntaxError(
                                $"Expected key-value pair, got: {line.Content}",
                                line.LineNo
                            );
                        }
                        _currentIndex++;
                        continue;
                    }
                    var tokenResult = SplitKeyValueToken(line.Content);
                    if (tokenResult is null)
                    {
                        if (_mode == "strict")
                        {
                            throw new InternalToonSyntaxError(
                                $"Invalid key-value syntax: {line.Content}",
                                line.LineNo
                            );
                        }
                        _currentIndex++;
                        continue;
                    }
                    var (keyToken, valueStr) = tokenResult.Value;
                    var key = keyToken.Clean;
                    InlineArrayInfo? inlineArray = !keyToken.WasQuoted
                        ? TryParseInlineArrayKey(key)
                        : null;
                    var targetKey = inlineArray?.BaseKey ?? key;
                    var allowPathExpansion = !keyToken.WasQuoted;
                    var treatAsInlineArray =
                        inlineArray.HasValue
                        && (!string.IsNullOrWhiteSpace(valueStr) || inlineArray.Value.Count == 0);
                    _currentIndex++;
                    object? value;
                    if (treatAsInlineArray)
                    {
                        value = ParseInlineArrayValues(
                            valueStr,
                            inlineArray!.Value.Count,
                            line.LineNo
                        );
                    }
                    else if (
                        _currentIndex < _lines.Count
                        && _lines[_currentIndex].Indent > baseIndent
                    )
                    {
                        int childIndent = _lines[_currentIndex].Indent;
                        value = ParseValue(childIndent);
                    }
                    else if (!string.IsNullOrWhiteSpace(valueStr))
                    {
                        value = ParseScalar(valueStr.Trim());
                    }
                    else
                    {
                        if (_mode == "strict" && _currentIndex >= _lines.Count)
                        {
                            throw new InternalToonSyntaxError(
                                $"Missing value for key: {key}",
                                line.LineNo
                            );
                        }
                        value = null;
                    }
                    AssignValue(result, targetKey, value, allowPathExpansion, line.LineNo);
                }
                return result;
            }

            private List<object?> ParseArray(int expectedIndent)
            {
                var result = new List<object?>();
                int baseIndent = expectedIndent;
                while (_currentIndex < _lines.Count)
                {
                    var line = _lines[_currentIndex];
                    if (line.Indent < baseIndent)
                    {
                        break;
                    }
                    if (
                        line.Indent != baseIndent
                        || !line.Content.StartsWith("-", StringComparison.Ordinal)
                    )
                    {
                        _currentIndex++;
                        continue;
                    }
                    string content =
                        line.Content.Length > 1
                            ? line.Content.Substring(1).TrimStart()
                            : string.Empty;
                    _currentIndex++;
                    object? value;
                    if (_currentIndex < _lines.Count && _lines[_currentIndex].Indent > baseIndent)
                    {
                        value = ParseValue(_lines[_currentIndex].Indent);
                    }
                    else
                    {
                        value = ParseScalar(content);
                    }
                    result.Add(value);
                }
                return result;
            }

            private sealed class TableHeaderInfo
            {
                public TableHeaderInfo(
                    string key,
                    bool allowPathExpansion,
                    int count,
                    List<string> fields,
                    string delimiter
                )
                {
                    Key = key;
                    AllowPathExpansion = allowPathExpansion;
                    Count = count;
                    Fields = fields;
                    Delimiter = delimiter;
                }

                public string Key { get; }
                public bool AllowPathExpansion { get; }
                public int Count { get; }
                public List<string> Fields { get; }
                public string Delimiter { get; }
            }

            private TableHeaderInfo? ParseTableHeader(string content)
            {
                content = content.Trim();
                if (!content.EndsWith(":", StringComparison.Ordinal))
                {
                    return null;
                }
                int bracketStart = content.IndexOf('[', StringComparison.Ordinal);
                int bracketEnd = content.IndexOf(']', bracketStart + 1);
                int braceStart = content.IndexOf('{', bracketEnd + 1);
                int braceEnd = content.IndexOf('}', braceStart + 1);
                if (
                    bracketStart < 0
                    || bracketEnd < 0
                    || braceStart < 0
                    || braceEnd < 0
                    || braceEnd > content.Length - 2
                )
                {
                    return null;
                }
                string rawKey = content.Substring(0, bracketStart).Trim();
                if (string.IsNullOrEmpty(rawKey))
                {
                    return null;
                }
                bool wasQuoted =
                    rawKey.Length >= 2 && rawKey.StartsWith('"') && rawKey.EndsWith('"');
                string cleanKey = UnquoteKey(rawKey);
                string bracketSegment = content.Substring(
                    bracketStart + 1,
                    bracketEnd - bracketStart - 1
                );
                string digitPart = new string(bracketSegment.TakeWhile(char.IsDigit).ToArray());
                if (string.IsNullOrEmpty(digitPart) || !int.TryParse(digitPart, out int count))
                {
                    return null;
                }
                string delimiterPart = bracketSegment.Substring(digitPart.Length);
                string delimiter = string.IsNullOrEmpty(delimiterPart)
                    ? DefaultTableDelimiter
                    : delimiterPart;
                string fieldsSegment = content.Substring(braceStart + 1, braceEnd - braceStart - 1);
                var fields = SplitEscapedRow(fieldsSegment, delimiter)
                    .Where(f => !string.IsNullOrWhiteSpace(f))
                    .ToList();
                if (fields.Count == 0)
                {
                    return null;
                }
                return new TableHeaderInfo(cleanKey, !wasQuoted, count, fields, delimiter);
            }

            private (List<Dictionary<string, object?>>, int) ParseTableFromHeader(
                int start,
                List<string> fields,
                int expectedLength,
                string delimiter,
                int indent
            )
            {
                var headerLine = _lines[start];
                int index = start + 1;
                var tableLines = new List<Line>();
                while (index < _lines.Count)
                {
                    var line = _lines[index];
                    if (line.Indent <= indent)
                    {
                        break;
                    }
                    tableLines.Add(line);
                    index++;
                }
                var rows = new List<Dictionary<string, object?>>(tableLines.Count);
                foreach (var line in tableLines)
                {
                    var values = SplitEscapedRow(line.Content.Trim(), delimiter);
                    if (values.Count == 0)
                    {
                        values = line
                            .Content.Trim()
                            .Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(v => v.Trim())
                            .ToList();
                    }
                    if (values.Count != fields.Count)
                    {
                        throw new InternalToonSyntaxError(
                            $"Expected {fields.Count} values in table row, got {values.Count}",
                            line.LineNo,
                            column: 1
                        );
                    }
                    var row = new Dictionary<string, object?>(fields.Count);
                    for (int i = 0; i < fields.Count; i++)
                    {
                        row[fields[i]] = ParseScalar(values[i]);
                    }
                    rows.Add(row);
                }
                if (rows.Count != expectedLength)
                {
                    throw new InternalToonSyntaxError(
                        $"Table header declares {expectedLength} rows, but found {rows.Count} rows",
                        headerLine.LineNo,
                        column: 1
                    );
                }
                return (rows, index);
            }

            private List<string> SplitEscapedRow(string line, string separator)
            {
                var result = new List<string>();
                var current = new StringBuilder();
                bool inQuotes = false;
                bool escape = false;
                for (int i = 0; i < line.Length; i++)
                {
                    char ch = line[i];
                    if (escape)
                    {
                        current.Append(ch);
                        escape = false;
                        continue;
                    }
                    if (ch == '\\')
                    {
                        current.Append(ch);
                        escape = true;
                        continue;
                    }
                    if (ch == '"')
                    {
                        current.Append(ch);
                        inQuotes = !inQuotes;
                        continue;
                    }
                    if (
                        !inQuotes
                        && i + separator.Length <= line.Length
                        && line.AsSpan(i, separator.Length).SequenceEqual(separator.AsSpan())
                    )
                    {
                        result.Add(current.ToString().Trim());
                        current.Clear();
                        i += separator.Length - 1;
                        continue;
                    }
                    current.Append(ch);
                }
                if (current.Length > 0)
                {
                    result.Add(current.ToString().Trim());
                }
                return result;
            }

            private readonly struct KeyToken
            {
                public KeyToken(string raw, string clean, bool wasQuoted)
                {
                    Raw = raw;
                    Clean = clean;
                    WasQuoted = wasQuoted;
                }

                public string Raw { get; }
                public string Clean { get; }
                public bool WasQuoted { get; }
            }

            private (KeyToken token, string? value)? SplitKeyValueToken(string line)
            {
                int colonIndex = line.IndexOf(':');
                if (colonIndex < 0)
                {
                    return null;
                }
                var span = line.AsSpan();
                string rawKey = span.Slice(0, colonIndex).TrimEnd().ToString();
                string? value =
                    colonIndex < span.Length - 1
                        ? span.Slice(colonIndex + 1).TrimStart().ToString()
                        : null;
                bool wasQuoted =
                    rawKey.Length >= 2 && rawKey.StartsWith('"') && rawKey.EndsWith('"');
                string cleanKey = UnquoteKey(rawKey);
                return (new KeyToken(rawKey, cleanKey, wasQuoted), value);
            }

            private readonly struct InlineArrayInfo
            {
                public InlineArrayInfo(string baseKey, int count)
                {
                    BaseKey = baseKey;
                    Count = count;
                }

                public string BaseKey { get; }
                public int Count { get; }
            }

            private InlineArrayInfo? TryParseInlineArrayKey(string key)
            {
                if (string.IsNullOrWhiteSpace(key) || !key.EndsWith("]", StringComparison.Ordinal))
                {
                    return null;
                }
                int bracketIndex = key.LastIndexOf('[');
                if (bracketIndex < 0)
                {
                    return null;
                }
                var span = key.AsSpan();
                var countSegment = span.Slice(bracketIndex + 1, span.Length - bracketIndex - 2);
                if (!int.TryParse(countSegment, out int count))
                {
                    return null;
                }
                string baseKey = span.Slice(0, bracketIndex).TrimEnd().ToString();
                if (string.IsNullOrWhiteSpace(baseKey))
                {
                    return null;
                }
                return new InlineArrayInfo(baseKey, count);
            }

            private List<object?> ParseInlineArrayValues(
                string? valueSegment,
                int expectedCount,
                int lineNo
            )
            {
                var tokens = string.IsNullOrWhiteSpace(valueSegment)
                    ? new List<string>()
                    : SplitEscapedRow(valueSegment.Trim(), DefaultTableDelimiter);
                if (tokens.Count != expectedCount)
                {
                    throw new InternalToonSyntaxError(
                        $"Inline array declares {expectedCount} values but found {tokens.Count}",
                        lineNo
                    );
                }
                var result = new List<object?>(tokens.Count);
                foreach (var token in tokens)
                {
                    result.Add(ParseScalar(token));
                }
                return result;
            }

            private void AssignValue(
                Dictionary<string, object?> target,
                string key,
                object? value,
                bool allowPathExpansion,
                int? lineNo
            )
            {
                if (!allowPathExpansion || !key.Contains('.', StringComparison.Ordinal))
                {
                    target[key] = value;
                    return;
                }
                var segments = new List<string>();
                var span = key.AsSpan();
                int start = 0;
                for (int i = 0; i <= span.Length; i++)
                {
                    if (i == span.Length || span[i] == '.')
                    {
                        if (i > start)
                        {
                            segments.Add(span.Slice(start, i - start).ToString());
                        }
                        start = i + 1;
                    }
                }
                if (segments.Count == 0)
                {
                    target[key] = value;
                    return;
                }
                var current = target;
                for (int i = 0; i < segments.Count - 1; i++)
                {
                    string segment = segments[i];
                    EnsureFoldableSegment(segment, lineNo);
                    if (!current.TryGetValue(segment, out var existing))
                    {
                        var next = new Dictionary<string, object?>();
                        current[segment] = next;
                        current = next;
                    }
                    else if (existing is Dictionary<string, object?> nested)
                    {
                        current = nested;
                    }
                    else
                    {
                        throw new InternalToonSyntaxError(
                            $"Path '{key}' conflicts with existing value at '{segment}'",
                            lineNo
                        );
                    }
                }
                string finalSegment = segments[^1];
                EnsureFoldableSegment(finalSegment, lineNo);
                if (
                    current.TryGetValue(finalSegment, out var existingFinal)
                    && existingFinal is Dictionary<string, object?>
                    && value is not Dictionary<string, object?>
                )
                {
                    throw new InternalToonSyntaxError(
                        $"Path '{key}' conflicts with existing nested object",
                        lineNo
                    );
                }
                current[finalSegment] = value;
            }

            private static void EnsureFoldableSegment(string segment, int? lineNo)
            {
                if (!FoldableSegmentRegex.IsMatch(segment))
                {
                    throw new InternalToonSyntaxError(
                        $"Invalid path segment '{segment}' for key folding",
                        lineNo
                    );
                }
            }

            private static bool LooksLikeMissingColon(string content)
            {
                if (string.IsNullOrWhiteSpace(content))
                {
                    return false;
                }
                string trimmed = content.Trim();
                if (
                    trimmed.StartsWith("-", StringComparison.Ordinal)
                    || trimmed.StartsWith("\"", StringComparison.Ordinal)
                )
                {
                    return false;
                }
                int separatorIndex = trimmed.IndexOfAny(new[] { ' ', '\t' });
                if (separatorIndex <= 0)
                {
                    return false;
                }
                string candidateKey = trimmed.Substring(0, separatorIndex);
                return FoldableSegmentRegex.IsMatch(candidateKey);
            }

            private static string UnquoteKey(string key)
            {
                if (key.StartsWith('"') && key.EndsWith('"'))
                {
                    try
                    {
                        return JsonSerializer.Deserialize<string>(key) ?? key;
                    }
                    catch
                    {
                        return key;
                    }
                }
                return key;
            }

            private object? ParseScalar(string content)
            {
                if (string.IsNullOrWhiteSpace(content))
                {
                    return null;
                }
                content = content.Trim();
                if (content == "[]")
                    return new List<object?>();
                if (content == "{}")
                    return new Dictionary<string, object?>();
                if (content == "true")
                    return true;
                if (content == "false")
                    return false;
                if (content == "null")
                    return null;
                object? number = GuessNumber(content);
                if (number is not null)
                {
                    return number;
                }
                if (content.StartsWith('"') && content.EndsWith('"'))
                {
                    try
                    {
                        return JsonSerializer.Deserialize<string>(content);
                    }
                    catch
                    {
                        return content;
                    }
                }
                return content;
            }

            private static object? GuessNumber(string token)
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    return null;
                }
                token = token.Trim();
                if (token.IndexOfAny(new[] { '.', 'e', 'E' }) >= 0)
                {
                    if (
                        double.TryParse(
                            token,
                            NumberStyles.Float,
                            CultureInfo.InvariantCulture,
                            out double d
                        )
                    )
                    {
                        return d;
                    }
                    return null;
                }
                if (
                    long.TryParse(
                        token,
                        NumberStyles.Integer,
                        CultureInfo.InvariantCulture,
                        out long l
                    )
                )
                {
                    return l;
                }
                return null;
            }
        }
    }
}
