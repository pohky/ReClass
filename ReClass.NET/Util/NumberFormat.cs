using System.Globalization;

namespace ReClassNET.Util;

public static class NumberFormat {
    public static NumberFormatInfo GuessNumberFormat(string input) {
        var commaIndex = input.IndexOf(',');
        var dotIndex = input.IndexOf('.');

        if (commaIndex > dotIndex) {
            return new NumberFormatInfo {
                NumberDecimalSeparator = ",",
                NumberGroupSeparator = "."
            };
        }

        return new NumberFormatInfo {
            NumberDecimalSeparator = ".",
            NumberGroupSeparator = ","
        };
    }
}
