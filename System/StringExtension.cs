namespace System;

public static class StringExtension
{
    public static Guid ToGuid(this string value)
    {
        return Guid.Parse(value);
    }

    public static string ToAscii(this string source)
    {
        var min = '\u0000';
        var max = '\u007F';
        return new string(source.Where(c => c >= min && c <= max).ToArray());
    }
}