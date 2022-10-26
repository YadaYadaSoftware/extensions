using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using YadaYada.TestUtilities;

namespace Logging.Test
{
    public class LoggerYadaYadaTest
    {
        [Fact]
        public void ScopeTest()
        {
            string expected = "1=2\r {\"CategoryName\":\"LoggerYadaYadaTest\"}";
            using var p = new MockPackage<LoggerYadaYada>(CreateLoggerMock);
            //using var scope = p.Target.BeginScope()
            string written = String.Empty;
            p.TargetMock.Setup(yada => yada.Write(It.IsAny<string>())).Callback((string s) => written = s);
            p.Target.LogTrace("{a}={b}", 1,2);
            written[0].Should().Be('1');
            written[1].Should().Be('=');
            written[2].Should().Be('2');
            written[3].Should().Be('\r');
            written[4].Should().Be(' ');
            written[5].Should().Be('{');
            written.Should().Be(expected);

        }

        private Mock<LoggerYadaYada> CreateLoggerMock()
        {
            return new Mock<LoggerYadaYada>(nameof(LoggerYadaYadaTest), new LambdaLoggerOptions { IncludeScopes = true, IncludeCategory = false, IncludeEventId = false, IncludeException = false, IncludeLogLevel = false, IncludeNewline = false});
        }
    }
}