using System.Xml.Linq;

namespace ReClass.Extensions;

public static class XAttributeExtensions {
    public static TEnum GetEnumValue<TEnum>(this XAttribute attribute) where TEnum : struct {
        TEnum @enum = default;
        if (attribute != null) {
            Enum.TryParse(attribute.Value, out @enum);
        }
        return @enum;
    }
}
