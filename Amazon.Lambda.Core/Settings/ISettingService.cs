using System.Runtime.CompilerServices;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public interface ISettingService
{
    Task<string> GetAsync([CallerMemberName] string name = "");
}