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
            Task mySqlTask = StartMySqlContainer(config.MySqlPort);
            Task msSqlTask = StartSqlServerContainer(config.MsSqlPassword, config.MsSqlPort);
            await Task.WhenAll(mySqlTask, msSqlTask);
        }

        [OneTimeTearDown]
        public async Task FixtureTearDown()
        {
            await Task.WhenAll(StopMsSqlContainer(), StopMySqlContainer());
        }

        private static async Task StopMsSqlContainer()
        {
            ProcessStartInfo killStartInfo = new ProcessStartInfo(
                DOCKER_PROCESS,
                $"exec {MS_SQL_CONTAINER_NAME} kill 1 || :");
            Process killProcess = Process.Start(killStartInfo);
            await killProcess.WaitForExitAsync();

            ProcessStartInfo stopStartInfo = new ProcessStartInfo(
                DOCKER_PROCESS,
                $"container stop {MY_SQL_CONTAINER_NAME}");
            Process stopProcess = Process.Start(stopStartInfo);
            await stopProcess.WaitForExitAsync();
        }

        private static async Task StopMySqlContainer()
        {
            ProcessStartInfo stopStartInfo = new ProcessStartInfo(
                DOCKER_PROCESS,
                $"container stop {MY_SQL_CONTAINER_NAME}");
            Process stopProcess = Process.Start(stopStartInfo);
            await stopProcess.WaitForExitAsync();
        }

        private static async Task StartMySqlContainer(int port)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(
                DOCKER_PROCESS,
                $"run --rm --name {MY_SQL_CONTAINER_NAME} -p {port}:3306 -e MYSQL_ALLOW_EMPTY_PASSWORD=yes -e TZ=Europe/London -d mysql:latest --max-connections=1000");
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

        private static async Task StartSqlServerContainer(string password, int port)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(
                DOCKER_PROCESS,
                $"run --rm -e \"ACCEPT_EULA=Y\" -e \"SA_PASSWORD={password}\" -e \"MSSQL_PID=Express\" -e \"TZ=Europe/London\" -p {port}:1433 -d --name={MS_SQL_CONTAINER_NAME} mcr.microsoft.com/mssql/server:2019-latest");
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
