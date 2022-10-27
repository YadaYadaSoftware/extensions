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
            string expected = "1=2\r {\"CategoryName\":\"LoggerYadaYadaTest\",\"a\":\"1\",\"b\":\"2\"}";
            using var p = new MockPackage<LoggerYadaYada>(CreateLoggerMock);
            string written = String.Empty;
            p.TargetMock.Setup(yada => yada.Write(It.IsAny<string>())).Callback((string s) => written = s);
            p.Target.LogTrace("{a}={b}", 1,2);
            written.Should().Be(expected);
        }

        [Fact]
        public void ScopeTest2()
        {
            string expected = "1=2\r {\"CategoryName\":\"LoggerYadaYadaTest\",\"a\":\"1\",\"b\":\"2\"}";
            using var p = new MockPackage<LoggerYadaYada>(CreateLoggerMock);
            string written = String.Empty;
            p.TargetMock.Setup(yada => yada.Write(It.IsAny<string>())).Callback((string s) => written = s);
            p.Target.LogTrace("{a}={b}", 1, 2);
            written.Should().Be(expected);
        }

        private Mock<LoggerYadaYada> CreateLoggerMock()
        {
            return new Mock<LoggerYadaYada>(nameof(LoggerYadaYadaTest), new LambdaLoggerOptions { IncludeScopes = true, IncludeCategory = false, IncludeEventId = false, IncludeException = false, IncludeLogLevel = false, IncludeNewline = false});
        }
    }
}