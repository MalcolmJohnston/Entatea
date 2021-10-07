using FluentMigrator;

namespace Entatea.Tests.Migrations
{
    [Migration(7)]
    public class M07_CreatePartitionedProductsTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("PartitionedProducts")
                .WithColumn("Id").AsInt32().PrimaryKey().NotNullable()
                .WithColumn("Name").AsString().Nullable()
                .WithColumn("IsForSale").AsBoolean().NotNullable()
                .WithColumn("Updated").AsDateTime().NotNullable()
                .WithColumn("Stock").AsInt32().NotNullable()
                .WithColumn("Price").AsDecimal().NotNullable();
        }
    }
}
