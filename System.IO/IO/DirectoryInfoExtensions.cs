namespace System.IO;

public static class DirectoryInfoExtensions
{
    public static DirectoryInfo CombineDirectory(this DirectoryInfo directoryInfo, params string[] paths)
    {
        var combine = new List<string> { directoryInfo.FullName };
        combine.AddRange(paths);
        return new DirectoryInfo(Path.Combine(combine.ToArray()));
    }
    public static FileInfo CombineFile(this DirectoryInfo directoryInfo, params string[] paths)
    {
        var combine = new List<string> { directoryInfo.FullName };
        combine.AddRange(paths);
        return new FileInfo(Path.Combine(combine.ToArray()));
    }
}