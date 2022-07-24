using Microsoft.Extensions.Logging;

namespace Amazon.Lambda.Core.Efs;

public class EfsMountTemporaryDirectoryFactory : ITemporaryDirectoryFactory
{
    public const string LocalMountPath = "/mnt/efs0";

    private readonly ILogger _logger;

    // ReSharper disable once MemberCanBePrivate.Global
    public const string EfsMountEnvironmentVariableName = "ElasticFileSystemMounts";

    public EfsMountTemporaryDirectoryFactory(ILoggerProvider loggerProvider)
    {
        this._logger = loggerProvider.CreateLogger(this.GetType().FullName);
    }
    public async Task<TemporaryDirectory> GetTemporaryDirectoryAsync()
    {
        using (_logger.AddMember(nameof(GetTemporaryDirectoryAsync)))
        {
            var newTemporaryDirectory = new DirectoryInfo(Path.Combine(LocalMountPath, Path.GetRandomFileName()));
            this._logger.LogDebug($"{nameof(newTemporaryDirectory)}-{newTemporaryDirectory}");
            newTemporaryDirectory.Create();
            return new TemporaryDirectory(newTemporaryDirectory);

        }        }
}