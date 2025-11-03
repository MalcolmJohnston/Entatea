using Entatea.Tests.Entities;
using FluentMigrator;

namespace Entatea.Tests.Migrations
{
    [Migration(10)]
    public class M10_HardDeleteTestTables : AutoReversingMigration
    {
        public override void Up()
        {
            

            IfDatabase(ProcessorId.SQLite)
                .Create.Table("SoftDeleteAssigned")
                .WithColumn("Code").AsString().PrimaryKey()
                .WithColumn("Value").AsString().NotNullable()
                .WithColumn("RecordStatus").AsInt16().NotNullable();

            IfDatabase(x => x != ProcessorId.SQLite)
                .Create.Table("SoftDeleteAssigned")
                    .InSchema("Entatea")
                    .WithColumn("Code").AsString().PrimaryKey()
                    .WithColumn("Value").AsString().NotNullable()
                    .WithColumn("RecordStatus").AsInt32().NotNullable();
        }
    }
}
