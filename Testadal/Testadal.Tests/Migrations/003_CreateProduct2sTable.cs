using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using FluentMigrator;

namespace Testadal.Tests.Migrations
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
