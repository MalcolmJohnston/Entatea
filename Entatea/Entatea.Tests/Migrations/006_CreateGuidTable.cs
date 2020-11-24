using FluentMigrator;

namespace Entatea.Tests.Migrations
{
    [Migration(6)]
    public class CreateGuidTable : AutoReversingMigration
    {
        public override void Up()
        {
            IfDatabase("SqlServer")
                .Create
                .Table("Uids")
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey().WithDefault(SystemMethods.NewGuid)
                .WithColumn("Value").AsString().NotNullable();
        }
    }
}
