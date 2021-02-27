using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

using Entatea.Tests.Helpers;
using MySql.Data;
using NUnit.Framework;

namespace Entatea.Tests
{
    [SetUpFixture]
    
    public class Setup
    {
        private const string MY_SQL_CONTAINER_NAME = "entatea-mysql";
        private const string DOCKER_PROCESS = "docker";

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            // start up a my-sql container
            ProcessStartInfo startInfo = new ProcessStartInfo(
                DOCKER_PROCESS,
                $"run --name {MY_SQL_CONTAINER_NAME} -p3306:3306 -e MYSQL_ALLOW_EMPTY_PASSWORD=yes -d mysql:latest --max-connections=1000");
            Process startProcess = Process.Start(startInfo);
            startProcess.WaitForExit();

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

        [OneTimeTearDown]
        public void FixtureTearDown()
        {
            // stop the my-sql container
            ProcessStartInfo stopStartInfo = new ProcessStartInfo(
                DOCKER_PROCESS,
                $"container stop {MY_SQL_CONTAINER_NAME}");
            Process stopProcess = Process.Start(stopStartInfo);
            stopProcess.WaitForExit();

            // remove the my-sql container
            ProcessStartInfo removeStartInfo = new ProcessStartInfo(
                DOCKER_PROCESS,
                $"container rm {MY_SQL_CONTAINER_NAME}");
            Process removeProcess = Process.Start(stopStartInfo);
            removeProcess.WaitForExit();
        }
    }
}
