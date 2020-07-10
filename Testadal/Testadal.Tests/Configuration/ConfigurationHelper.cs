using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Testadal.Tests.Configuration
{
    public static class ConfigurationHelper
    {
        private static readonly object lockObject = new object();

        private static TestConfiguration config;

        public static TestConfiguration GetTestConfiguration()
        {
            lock(lockObject)
            {
                if (config == null)
                {
                    config = new TestConfiguration();
                    new ConfigurationBuilder()
                        .SetBasePath(TestContext.CurrentContext.TestDirectory)
                        .AddJsonFile("appsettings.json", optional: true)
                        .AddUserSecrets<TestConfiguration>()
                        .AddEnvironmentVariables()
                        .Build()
                        .GetSection("TestadalTests")
                        .Bind(config);
                }
            }

            return config;
        }
    }
}
