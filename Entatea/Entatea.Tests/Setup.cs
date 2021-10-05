using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Entatea.Tests.Configuration;
using Entatea.Tests.Helpers;
using NUnit.Framework;

namespace Entatea.Tests
{
    [SetUpFixture]
    
    public class Setup
    {
        private const string MY_SQL_CONTAINER_NAME  = "entatea-mysql";
        private const string MS_SQL_CONTAINER_NAME  = "entatea-sqlserver";
        private const string DOCKER_PROCESS         = "docker";

        [OneTimeSetUp]
        public async Task FixtureSetup()
        {
            TestConfiguration config = ConfigurationHelper.GetTestConfiguration();
            await Task.WhenAll(this.StartMySqlContainer(config.MySqlPort), this.StartSqlServerContainer(config.MsSqlPassword, config.MsSqlPort));
        }

        [OneTimeTearDown]
        public async Task FixtureTearDown()
        {
            await Task.WhenAll(this.StopContainer(MY_SQL_CONTAINER_NAME), this.StopContainer(MS_SQL_CONTAINER_NAME));
        }

        private async Task StopContainer(string containerName)
        {
            // stop the my-sql container
            ProcessStartInfo stopStartInfo = new ProcessStartInfo(
                DOCKER_PROCESS,
                $"container stop {containerName}");
            Process stopProcess = Process.Start(stopStartInfo);
            await stopProcess.WaitForExitAsync();

            // remove the my-sql container
            ProcessStartInfo removeStartInfo = new ProcessStartInfo(
                DOCKER_PROCESS,
                $"container rm {containerName}");
            Process removeProcess = Process.Start(removeStartInfo);
            await removeProcess.WaitForExitAsync();
        }

        private async Task StartMySqlContainer(int port)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(
                DOCKER_PROCESS,
                $"run --name {MY_SQL_CONTAINER_NAME} -p{port}:3306 -e MYSQL_ALLOW_EMPTY_PASSWORD=yes -e TZ=Europe/London -d mysql:latest --max-connections=1000");
            Process startProcess = Process.Start(startInfo);
            await startProcess.WaitForExitAsync();

            string connectionString = MySqlTestHelper.GetMySqlConnectionString();
            while (true)
            {
                try
                {
                    MySqlTestHelper.OpenConnection(connectionString);
                }
                catch
                {
                    Console.WriteLine("Waiting for My SQL to initialise...");
                    Thread.Sleep(5000);
                    continue;
                }

                break;
            }
        }

        private async Task StartSqlServerContainer(string password, int port)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(
                DOCKER_PROCESS,
                $"run --name {MS_SQL_CONTAINER_NAME} -e \"ACCEPT_EULA=Y\" -e \"SA_PASSWORD={password}\" -e \"MSSQL_PID=Express\" -e \"TZ=Europe/London\" -p{port}:1433 -d --name=sql mcr.microsoft.com/mssql/server:latest");
            Process startProcess = Process.Start(startInfo);
            await startProcess.WaitForExitAsync();

            string connectionString = MsSqlTestHelper.GetMsSqlConnectionString();
            while (true)
            {
                try
                {
                    MsSqlTestHelper.OpenConnection(connectionString);
                }
                catch
                {
                    Console.WriteLine("Waiting for My SQL to initialise...");
                    Thread.Sleep(5000);
                    continue;
                }

                break;
            }
        }
    }
}
