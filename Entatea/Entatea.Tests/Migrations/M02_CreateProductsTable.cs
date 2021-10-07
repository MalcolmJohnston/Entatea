using FluentMigrator;

namespace Entatea.Tests.Migrations
{
    [Migration(2)]
    public class M02_CreateProductsTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("Products")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Name").AsString().Nullable()
                .WithColumn("IsForSale").AsBoolean().NotNullable()
                .WithColumn("Updated").AsDateTime().NotNullable()
                .WithColumn("Stock").AsInt32().NotNullable()
                .WithColumn("Price").AsDecimal().NotNullable();
        }
    }
}
