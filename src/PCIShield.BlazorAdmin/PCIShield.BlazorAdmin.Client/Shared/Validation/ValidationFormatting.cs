using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;

using FluentValidation.Results;

using System.Collections.Generic;
using System.Linq;
using FluentValidation;

namespace PCIShield.BlazorAdmin.Client.Shared.Validation;

public static class ValidationFormatting
{
    public static string ToDetailedString<T>(ValidationResult vr, T root, int valueMaxLen = 160)
    {
        static string? TryGet(IDictionary<string, object>? d, string key)
            => d != null && d.TryGetValue(key, out var v) ? v?.ToString() : null;

        static string Trunc(object? v, int max)
        {
            if (v is null) return "∅";
            var s = v.ToString() ?? "∅";
            return s.Length <= max ? s : s[..max] + "…";
        }

        static string FormatSeqPreview(object? v, int max = 160)
        {
            if (v is null) return "∅";
            if (v is string s) return s.Length <= max ? s : s[..max] + "…";
            if (v is IEnumerable seq && v is not string)
            {
                var preview = new List<string>(4);
                foreach (var item in seq)
                {
                    if (preview.Count == 3) { preview.Add("…"); break; }
                    preview.Add(item?.ToString() ?? "∅");
                }
                var text = "[" + string.Join(", ", preview) + "]";
                return text.Length <= max ? text : text[..max] + "…";
            }
            return Trunc(v, max);
        }

        string Describe(ValidationFailure f)
        {
            var p = f.FormattedMessagePlaceholderValues;
            var path = TryGet(p, "PropertyPath") ?? f.PropertyName ?? string.Empty;
            var attempted = TryGet(p, "PropertyValue") ?? f.AttemptedValue?.ToString();
            var index = TryGet(p, "CollectionIndex");
            var errorCode = f.ErrorCode;
            var sev = f.Severity.ToString();
            if (!string.IsNullOrWhiteSpace(index) && !path.Contains($"[{index}]"))
                path += $"[{index}]";
            ResolveContext(root!, path,
                out var nodeValue,
                out var parent,
                out var collectionCount,
                out var parentIdSnippet);
            var hints = new List<string>();
            if (f.ErrorMessage.Contains("At least one", StringComparison.OrdinalIgnoreCase) ||
                (f.ErrorCode?.EndsWith("_REQUIRED", StringComparison.OrdinalIgnoreCase) ?? false))
            {
                if (collectionCount != null)
                    hints.Add($"FoundCount={collectionCount}");
            }
            if (f.ErrorMessage.Contains("invalid format", StringComparison.OrdinalIgnoreCase) ||
                (f.ErrorCode?.EndsWith("_FORMAT", StringComparison.OrdinalIgnoreCase) ?? false))
            {
                if (!string.IsNullOrWhiteSpace(attempted))
                    hints.Add($"Attempted=\"{Trunc(attempted, valueMaxLen)}\"");
            }
            if (!string.IsNullOrWhiteSpace(parentIdSnippet))
                hints.Add(parentIdSnippet!);
            if (nodeValue.exists)
                hints.Add($"ValuePreview={FormatSeqPreview(nodeValue.value, valueMaxLen)}");

            var hintText = hints.Count > 0 ? " | " + string.Join(" | ", hints) : string.Empty;

            return $"PropertyPath: \"{path}\" | Error: {f.ErrorMessage} | Code: {errorCode} | Severity: {sev}{hintText}";
        }
        static string NormalizePath(string path) => Regex.Replace(path, @"\[\d+\]", "[]");
        var enriched = vr.Errors.Select(f =>
        {
            var p = f.FormattedMessagePlaceholderValues;
            var path = (p != null && p.TryGetValue("PropertyPath", out var pv) ? pv?.ToString() : f.PropertyName) ?? string.Empty;
            var index = (p != null && p.TryGetValue("CollectionIndex", out var iv) ? iv?.ToString() : null);
            if (!string.IsNullOrWhiteSpace(index) && !path.Contains($"[{index}]")) path += $"[{index}]";
            ResolveContext(root!, path, out var _, out var _, out var _, out var parentId);

            return new
            {
                Failure = f,
                Path = path,
                Normalized = NormalizePath(path),
                ParentId = parentId
            };
        });
        var groups = enriched.GroupBy(x => (x.Normalized, x.Failure.ErrorCode ?? x.Failure.ErrorMessage));

        var lines = groups.Select(g =>
        {
            var first = g.First();
            var primary = Describe(first.Failure);
            var normalizedPrimary = Regex.Replace(
                primary,
                $"PropertyPath: \"{Regex.Escape(first.Path)}\"",
                $"PropertyPath: \"{first.Normalized}\""
            );
            var extraCount = g.Count() - 1;
            if (extraCount <= 0) return normalizedPrimary;

            var parents = g.Select(x => x.ParentId)
                           .Where(s => !string.IsNullOrWhiteSpace(s))
                           .Distinct()
                           .Take(3);

            var parentHint = parents.Any()
                ? $" | AffectedParents={string.Join("; ", parents)}"
                : string.Empty;

            return $"{normalizedPrimary}  ×{g.Count()}{parentHint}";
        });

        return string.Join(Environment.NewLine, lines);
    }
    public static void ResolveContext(
        object root,
        string path,
        out (bool exists, object? value) node,
        out object? parent,
        out int? collectionCount,
        out string? parentIdentifierSnippet)
    {
        node = (false, null);
        parent = null;
        collectionCount = null;
        parentIdentifierSnippet = null;

        try
        {
            IEnumerable<Token> tokens = ParsePath(path);
            object? current = root;
            object? prev = null;

            foreach (Token t in tokens)
            {
                prev = current;
                if (current is null) break;

                if (t.IsIndex)
                {
                    if (current is IEnumerable enumerable && current is not string)
                    {
                        IList list = ToIList(enumerable);
                        current = t.Index >= 0 && t.Index < list.Count ? list[t.Index] : null;
                    }
                    else
                    {
                        current = null;
                    }
                }
                else
                {
                    PropertyInfo? prop = current.GetType().GetProperty(t.Name!,
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    current = prop?.GetValue(current);
                }
            }

            node = (true, current);
            parent = prev;
            if (node.value is IEnumerable seq && node.value is not string)
                collectionCount = ToIList(seq).Count;
            parentIdentifierSnippet = BuildIdSnippet(parent);
        }
        catch
        {
        }
    }

    private static string? BuildIdSnippet(object? parent)
    {
        if (parent is null) return null;

        string[] candidates = new[]
        {
            "Name", "Code", "Id", "AssetName", "AssetCode", "AssetId", "AssessmentCode", "AssessmentId",
            "ChannelName", "ChannelCode", "PaymentChannelId", "MerchantName", "MerchantCode", "MerchantId",
            "Hostname", "IPAddress"
        };

        List<string> hits = new();
        Type t = parent.GetType();
        foreach (string c in candidates)
        {
            PropertyInfo? p = t.GetProperty(c, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (p?.CanRead == true)
            {
                object? val = p.GetValue(parent);
                if (val != null)
                {
                    hits.Add($"{c}={val}");
                    if (hits.Count == 2) break;
                }
            }
        }

        return hits.Count > 0 ? "Parent{" + string.Join(", ", hits) + "}" : null;
    }

    private static IList ToIList(IEnumerable seq)
    {
        if (seq is IList il) return il;

        List<object?> list = new();
        foreach (object? x in seq) list.Add(x);
        return list;
    }

    private static IEnumerable<Token> ParsePath(string path)
    {
        foreach (string segment in path.Split('.',
                     StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            string s = segment;
            string name = s;
            List<int> indexStack = new();
            int bracket;
            while ((bracket = name.IndexOf('[')) >= 0)
            {
                string baseName = name[..bracket];
                string rest = name[(bracket + 1)..];
                int close = rest.IndexOf(']');
                if (close < 0) break;

                string indexText = rest[..close];
                if (int.TryParse(indexText, out int idx))
                    indexStack.Add(idx);

                name = baseName + rest[(close + 1)..];
            }

            if (!string.IsNullOrWhiteSpace(name))
                yield return new Token { Name = name };

            foreach (int idx in indexStack)
                yield return new Token { IsIndex = true, Index = idx };
        }
    }

    private readonly struct Token
    {
        public string? Name { get; init; }
        public bool IsIndex { get; init; }
        public int Index { get; init; }
    }
}