using System.Collections.Generic;

using FluentMigrator;

namespace Entatea.Tests.Migrations
{
    [Migration(4)]
    public class CreateTestResolverTables : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("TestResolvers")
                .WithColumn("TestResolverId").AsInt32().PrimaryKey().Identity()
                .WithColumn("ResolverValue").AsString().Nullable();

            Create.Table("test_resolvers")
                .WithColumn("test_resolver_id").AsInt32().PrimaryKey().Identity()
                .WithColumn("resolver_value").AsString().Nullable();

            Insert.IntoTable("TestResolvers")
                .Row(new { ResolverValue = "Default" });
            Insert.IntoTable("test_resolvers")
                .Row(new Dictionary<string, object>()
                {
                    { "resolver_value", "Underscore" }
                });
        }
    }
}
