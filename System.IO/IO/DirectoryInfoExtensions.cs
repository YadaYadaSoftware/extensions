namespace System.IO;

public static class DirectoryInfoExtensions
{
    public static DirectoryInfo Combine(this DirectoryInfo directoryInfo, params string[] paths)
    {
        var combine = new List<string> {directoryInfo.FullName};
        combine.AddRange(paths);
        return new DirectoryInfo(Path.Combine(combine.ToArray()));
    }
}