using FluentMigrator;

namespace Entatea.Tests.Migrations
{
    [Migration(9)]
    public class M09_SoftDeleteShortTestTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("SoftDeleteShortTest")
                .WithColumn("SoftDeleteId").AsInt32().PrimaryKey().Identity()
                .WithColumn("Value").AsString().NotNullable()
                .WithColumn("RecordStatus").AsInt16().NotNullable();
        }
    }
}
