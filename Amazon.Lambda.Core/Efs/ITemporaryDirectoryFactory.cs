namespace Amazon.Lambda.Core.Efs;

public interface ITemporaryDirectoryFactory
{
    Task<TemporaryDirectory> GetTemporaryDirectoryAsync();
}