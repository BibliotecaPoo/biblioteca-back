namespace Biblioteca.Core.Extensions;

public static class StringExtension
{
    public static string? SomenteNumeros(this string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : string.Join("", System.Text.RegularExpressions.Regex.Split(value, @"[^\d]"));
    }
}