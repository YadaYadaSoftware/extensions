using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace YadaYada.Library.Extensions;

public class ClaimsPrincipalExtensionTest
{
    [Fact]
    public void AddTwoClaims()
    {
        var serviceCollection = new ServiceCollection();
        var loggerProvider = new Mock<ILoggerProvider>();
        loggerProvider.Setup(_ => _.CreateLogger(It.IsAny<string>())).Returns(new Mock<ILogger>().Object);
        serviceCollection.AddSingleton<ILoggerProvider>(loggerProvider.Object);
        serviceCollection.AddClaim(ClaimsExtensions.ClaimTypes.TenantId, Guid.NewGuid().ToString());
        serviceCollection.AddClaim(ClaimsExtensions.ClaimTypes.AmazonSellerId, Guid.NewGuid().ToString());
        serviceCollection.Where(_ => _.ServiceType == typeof(Claim)).Should().HaveCount(2);
        var provider = serviceCollection.BuildServiceProvider();
    }
}