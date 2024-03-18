using System.ComponentModel;

namespace Biblioteca.Core.Extensions;

public static class EnumExtension
{
    public static string ToDescriptionString(this System.Enum value)
    {
        var attributes = (DescriptionAttribute[])value
            .GetType()
            .GetField(value.ToString())
            ?.GetCustomAttributes(typeof(DescriptionAttribute), false)!;

        return attributes.Length > 0 ? attributes[0].Description : string.Empty;
    }
}