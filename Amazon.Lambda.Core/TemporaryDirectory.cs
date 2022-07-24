namespace Amazon.Lambda.Core;

public sealed class TemporaryDirectory : IDisposable
{

    public TemporaryDirectory(DirectoryInfo directoryInfo)
    {
        this.DirectoryInfo = directoryInfo;
    }

    public DirectoryInfo DirectoryInfo { get; }
    public void Dispose()
    {
        try
        {
            if (this.DirectoryInfo.Exists && !this.DirectoryInfo.FullName.StartsWith("/tmp", StringComparison.InvariantCultureIgnoreCase))
            {
                this.DirectoryInfo.Delete(true);
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}