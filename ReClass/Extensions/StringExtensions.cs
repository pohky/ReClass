using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ReClass.Extensions;

public static class StringExtension {
    private static readonly Regex hexadecimalValueRegex = new("^(0x|h)?([0-9A-F]+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    [DebuggerStepThrough]
    public static bool IsPrintable(this char c) => ((' ' <= c && c <= '~') || ('\xA1' <= c && c <= '\xFF')) && c != '\xFFFD' /* Unicode REPLACEMENT CHARACTER � */;

    [DebuggerStepThrough]
    public static IEnumerable<char> InterpretAsSingleByteCharacter(this IEnumerable<byte> source) {
        return source.Select(b => (char)b);
    }

    [DebuggerStepThrough]
    public static IEnumerable<char> InterpretAsDoubleByteCharacter(this IEnumerable<byte> source) {
        var bytes = source.ToArray();
        var chars = new char[bytes.Length / 2];
        Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
        return chars;
    }

    [DebuggerStepThrough]
    public static bool IsPrintableData(this IEnumerable<char> source) {
        return CalculatePrintableDataThreshold(source) >= 1.0f;
    }

    [DebuggerStepThrough]
    public static bool IsLikelyPrintableData(this IEnumerable<char> source) {
        return CalculatePrintableDataThreshold(source) >= 0.75f;
    }

    [DebuggerStepThrough]
    public static float CalculatePrintableDataThreshold(this IEnumerable<char> source) {
        var doCountValid = true;
        var countValid = 0;
        var countAll = 0;

        foreach (var c in source) {
            countAll++;

            if (doCountValid) {
                if (c.IsPrintable()) {
                    countValid++;
                } else {
                    doCountValid = false;
                }
            }
        }

        if (countAll == 0) {
            return 0.0f;
        }

        return countValid / (float)countAll;
    }

    [DebuggerStepThrough]
    public static string LimitLength(this string s, int length) {
        if (s.Length <= length) {
            return s;
        }
        return s.Substring(0, length);
    }
    public static bool TryGetHexString(this string s, out string value) {
        var match = hexadecimalValueRegex.Match(s);
        value = match.Success ? match.Groups[2].Value : null;

        return match.Success;
    }
}