using FluentMigrator;

namespace Entatea.Tests.Migrations
{
    [Migration(5)]
    public class CreateDiscriminatorsTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("Discriminators")
                .WithColumn("DiscriminatorId").AsInt32().PrimaryKey().Identity()
                .WithColumn("DiscriminatorType").AsInt32().NotNullable()
                .WithColumn("Name").AsString().NotNullable();
        }
    }
}
