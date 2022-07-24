using FluentAssertions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public class EnvironmentVariableSettingServiceTest
{
    [Fact]
    public void NullThrows()
    {
        var t = new EnvironmentVariableSettingService();
        // ReSharper disable once AssignNullToNotNullAttribute
        t.Invoking(_ => _.GetAsync(null)).Should().ThrowAsync<ArgumentNullException>();
        t.Invoking(_ => _.GetAsync(string.Empty)).Should().ThrowAsync<ArgumentNullException>();
        t.Invoking(_ => _.GetAsync(Guid.NewGuid().ToString())).Should().ThrowAsync<KeyNotFoundException>();
    }

}