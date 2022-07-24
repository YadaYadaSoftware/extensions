namespace Amazon.Lambda.Core;

public interface ITemporaryDirectoryFactory
{
    Task<TemporaryDirectory> GetTemporaryDirectoryAsync();
}