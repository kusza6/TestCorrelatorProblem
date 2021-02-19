using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using PbLogging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.TestCorrelator;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.TestCorrelator()
                .WriteTo.File(@"C:\logs\test.txt")
                .CreateLogger();
            using (TestCorrelator.CreateContext())
            {
                var hostBuilder = new WebHostBuilder();
                hostBuilder
                    .UseSerilog()
                    .UseStartup<TestCorrelatorControllerProblem.Startup>();
                
                var testAppServer = new TestServer(hostBuilder);
                var testAppClient = testAppServer.CreateClient();

                var results = await testAppClient.GetAsync("weatherforecast");
                var logEvent = TestCorrelator.GetLogEventsFromCurrentContext();
            }
        }
    } }
