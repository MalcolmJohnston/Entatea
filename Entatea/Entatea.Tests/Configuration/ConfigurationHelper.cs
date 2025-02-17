using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Entatea.Tests.Configuration
{
    public static class ConfigurationHelper
    {
        private static readonly object lockObject = new();

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
                        .GetSection("EntateaTests")
                        .Bind(config);
                }
            }

            return config;
        }
    }
}
