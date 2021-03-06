using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoNancyTests
{
    using Nancy;
    using Nancy.Testing;
    using NLog;
    using NLog.Config;
    using NLog.Targets;
    using Xunit;
    using Xunit.Extensions;

    public class RequestLoggingTests
    {
        private Nancy.Testing.Browser sut;
        private NLog.Targets.MemoryTarget actualLog;
        private const string infoRequestlogger = "|INFO|RequestLogger";
        private const string errorRequestlogger = "|ERROR|RequestLogger";

        public RequestLoggingTests()
        {
            sut = new Browser(new ToDoNancy.Bootstrapper());
            OverrideNLogConfiguration();
        }

        private void OverrideNLogConfiguration()
        {
            actualLog = new MemoryTarget();
            actualLog.Layout += "|${exception}";
            NLog.Config.SimpleConfigurator.ConfigureForTargetLogging(actualLog, LogLevel.Info);
        }

        [Xunit.Extensions.Theory]
        [Xunit.Extensions.InlineData("/")]
        [InlineData("/todos/")]
        [InlineData("/shouldnotbefound/")]
        public void ShouldLogIncomingRequests(string path)
        {
            sut.Get(path);

            Assert.True(TryFindExptedInfoLog(actualLog, "Handling request GET \"" + path + "\""));
        }
        
        [Theory]
        [InlineData("/", HttpStatusCode.OK)]
        [InlineData("/todos/", HttpStatusCode.OK)]
        [InlineData("/shouldnotbefound/", HttpStatusCode.NotFound)]
        public void ShouldLogStatusCodeOffResponses(string path, HttpStatusCode expectedStatusCode)
        {
            sut.Get(path);

            Assert.True(TryFindExptedInfoLog(actualLog, "Responding " + expectedStatusCode + " to GET \"" + path + "\""));
        }

        [Fact]
        public void ShouldLogMethod_post()
        {
            sut.Post("/");

            Assert.True(TryFindExptedInfoLog(actualLog, "POST"));
        }

        [Fact]
        public void ShoudLogMethod_put()
        {
            sut.Put("/");

            Assert.True(TryFindExptedInfoLog(actualLog, "PUT"));
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/todos/")]
        public void ShouldNotLogErrorOnSuccessfulRequest(string path)
        {
            sut.Get(path);

            Assert.False(TryFindExptedErrorLog(actualLog, ""));
        }

        [Fact]
        public void ShouldLogErrorOnFailingRequest()
        {
            try{
                sut.Delete("/todos/illegal_item_id");
            }
            
            catch{}
            finally
            {
                Assert.True(TryFindExptedErrorLog(actualLog, "入力文字列の形式が正しくありません。"));
            }
        }


        //----------------
        private static bool TryFindExptedInfoLog(MemoryTarget actualLog, string expected)
        {
            return TryFindExptedLogAtExpectedLevel(actualLog, expected, infoRequestlogger);
        }

        private static bool TryFindExptedErrorLog(MemoryTarget actualLog, string expected)
        {
            return TryFindExptedLogAtExpectedLevel(actualLog, expected, errorRequestlogger);
        }

        private static bool TryFindExptedLogAtExpectedLevel(
            MemoryTarget actualLog, string expected, string requestloggerLevel)
        {
            var tryFindExptedLog = actualLog.Logs
                .Where(s => s.Contains(requestloggerLevel))
                .FirstOrDefault(s => s.Contains(expected));

            if (tryFindExptedLog != null) return true;

            Console.WriteLine(
                "\"{0}\" not found in log filtered by \"{1}\":",
                expected, requestloggerLevel);

            Console.WriteLine(
                actualLog.Logs
                .Aggregate("[\n\t{0}\n", (acc, s1) => string.Format(acc, s1 + "\n\t{0}" )));

            return false;
        }
    }
}
