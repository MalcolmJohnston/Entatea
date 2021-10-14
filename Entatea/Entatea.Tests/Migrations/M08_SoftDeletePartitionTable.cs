using FluentMigrator;

namespace Entatea.Tests.Migrations
{
    [Migration(8)]
    public class M08_SoftDeletePartitionTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("SoftDeletePartition")
                .WithColumn("SoftDeleteId").AsInt32().PrimaryKey()
                .WithColumn("Value").AsString().NotNullable()
                .WithColumn("RecordStatus").AsInt32().NotNullable();
        }
    }
}
