using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using FluentMigrator;

namespace Testadal.Tests.Migrations
{
    [Migration(2)]
    public class CreateProductsTable : AutoReversingMigration
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
