namespace System;

public static class BooleanExtension
{
    // ReSharper disable once InconsistentNaming
    public static string ToYN(this bool value)
    {
        if (value) return "Y";
        return "N";
    }
}