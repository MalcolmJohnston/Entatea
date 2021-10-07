using FluentMigrator;

namespace Entatea.Tests.Migrations
{
    [Migration(6)]
    public class M06_CreateGuidTable : AutoReversingMigration
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
