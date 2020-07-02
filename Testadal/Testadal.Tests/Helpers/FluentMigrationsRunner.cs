using Microsoft.Extensions.DependencyInjection;

using FluentMigrator.Runner;

namespace Testadal.Tests.Helpers
{
    public static class FluentMigrationsRunner
    {
        public static void UpLocalDb(string connectionString)
        {
            new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddSqlServer()
                    .WithGlobalConnectionString(connectionString)
                    // Define the assembly containing the migrations
                    .ScanIn(typeof(Migrations.CreateSchema).Assembly).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .BuildServiceProvider(false)
                .GetRequiredService<IMigrationRunner>()
                .MigrateUp();
        }

        public static void UpMySql(string connectionString)
        {
            new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddMySql5()
                    .WithGlobalConnectionString(connectionString)
                    // Define the assembly containing the migrations
                    .ScanIn(typeof(Migrations.CreateSchema).Assembly).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .BuildServiceProvider(false)
                .GetRequiredService<IMigrationRunner>()
                .MigrateUp();
        }
    }
}
