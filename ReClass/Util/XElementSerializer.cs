using System.Globalization;
using System.Xml.Linq;
using ReClass.Extensions;

namespace ReClass.Util;

public static class XElementSerializer {
    public static bool TryRead(XContainer element, string name, Action<XElement> iff) {
        var target = element.Element(name);
        if (target != null) {
            iff(target);

            return true;
        }

        return false;
    }

    public static bool ToBool(XElement value) => (bool?)value ?? false;
    public static int ToInt(XElement value) => (int?)value ?? 0;
    public static string ToString(XElement value) => value.Value;
    public static Color ToColor(XElement value) => Color.FromArgb((int)(0xFF000000 | int.Parse(value.Value, NumberStyles.HexNumber)));
    public static Dictionary<string, string> ToDictionary(XContainer value) => value.Elements().ToDictionary(e => e.Name.ToString(), e => e.Value);

    public static XElement ToXml(string name, bool value) => new(name, value);
    public static XElement ToXml(string name, int value) => new(name, value);
    public static XElement ToXml(string name, string value) => new(name, value);
    public static XElement ToXml(string name, Color value) => new(name, $"{value.ToRgb():X6}");
    public static XElement ToXml(string name, Dictionary<string, string> value) => new(name, value.Select(kv => new XElement(kv.Key, kv.Value)));
}
