using System.ComponentModel;
using System.Reflection;

namespace ReClassNET.Controls;

public class EnumDescriptionDisplay<TEnum> where TEnum : struct {
    public TEnum Value { get; internal set; }
    public string Description { get; internal set; }

    public static List<EnumDescriptionDisplay<TEnum>> Create() {
        return CreateExact(Enum.GetValues(typeof(TEnum)).Cast<TEnum>());
    }

    public static List<EnumDescriptionDisplay<TEnum>> CreateExact(IEnumerable<TEnum> include) {
        return include
            .Select(value => new EnumDescriptionDisplay<TEnum> {
                Description = GetDescription(value),
                Value = value
            })
            .OrderBy(item => item.Value)
            .ToList();
    }

    public static List<EnumDescriptionDisplay<TEnum>> CreateExclude(IEnumerable<TEnum> exclude) {
        return Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .Except(exclude)
            .Select(value => new EnumDescriptionDisplay<TEnum> {
                Description = GetDescription(value),
                Value = value
            })
            .OrderBy(item => item.Value)
            .ToList();
    }

    private static string GetDescription(TEnum value) {
        return value.GetType().GetField(value.ToString()).GetCustomAttribute<DescriptionAttribute>()?.Description ?? value.ToString();
    }
}
