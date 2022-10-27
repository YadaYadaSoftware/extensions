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
            string expected = "1=2 {\"CategoryName\":\"LoggerYadaYadaTest\",\"a\":\"1\",\"b\":\"2\"}";
            using var p = new MockPackage<LoggerYadaYada>(CreateLoggerMock);
            string written = String.Empty;
            p.TargetMock.Setup(yada => yada.Write(It.IsAny<string>())).Callback((string s) => written = s);
            p.Target.LogTrace("{a}={b}", 1,2);
            written.Replace("\r", string.Empty) .Should().Be(expected);
        }

        [Fact]
        public void BeginScopeTest()
        {
            string expected = "x {\"CategoryName\":\"LoggerYadaYadaTest\",\"Scope\":\"Scope1\"}";
            using var p = new MockPackage<LoggerYadaYada>(CreateLoggerMock);
            string written = String.Empty;
            p.TargetMock.Setup(yada => yada.Write(It.IsAny<string>())).Callback((string s) => written = s);
            using var scope = p.Target.BeginScope("Scope1");
            p.Target.LogTrace("x");
            written.Replace("\r", string.Empty).Should().Be(expected);
        }

        [Fact]
        public void BeginScope2Test()
        {
            string expected = "x {\"CategoryName\":\"LoggerYadaYadaTest\",\"Scope\":\"Scope1, Scope2\"}";
            using var p = new MockPackage<LoggerYadaYada>(CreateLoggerMock);
            string written = String.Empty;
            p.TargetMock.Setup(yada => yada.Write(It.IsAny<string>())).Callback((string s) => written = s);
            using var scope1 = p.Target.BeginScope("Scope1");
            using var scope2 = p.Target.BeginScope("Scope2");
            p.Target.LogTrace("x");
            written.Replace("\r", string.Empty).Should().Be(expected);
        }

        private Mock<LoggerYadaYada> CreateLoggerMock()
        {
            return new Mock<LoggerYadaYada>(nameof(LoggerYadaYadaTest), new LambdaLoggerOptions { IncludeScopes = true, IncludeCategory = false, IncludeEventId = false, IncludeException = false, IncludeLogLevel = false, IncludeNewline = false});
        }
    }
}