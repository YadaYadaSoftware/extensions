namespace System.Printing;

internal static class ZplConstants
{
    internal const string FEED = "^XA^AA^FD ^FS^XZ";
    internal const string PRINT_MODE_FMT = "^XA^MM{0}^XZ";
    internal const string PAUSE = "^XA~PP^XZ";
    internal const string UNPAUSE = "^XA~PS^XZ";
    internal const string DELAYED_CUT = "~JK";
}