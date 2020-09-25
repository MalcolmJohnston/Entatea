using FluentMigrator;

namespace Entatea.Tests.Migrations
{
    [Migration(3)]
    public class CreateProduct2sTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("Product2s")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Name").AsString().Nullable()
                .WithColumn("IsForSale").AsBoolean().Nullable()
                .WithColumn("Updated").AsDateTime().Nullable()
                .WithColumn("Stock").AsInt32().Nullable()
                .WithColumn("Price").AsDecimal().Nullable();
        }
    }
}
