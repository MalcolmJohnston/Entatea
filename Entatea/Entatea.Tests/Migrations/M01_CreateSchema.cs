﻿using FluentMigrator;

namespace Entatea.Tests.Migrations
{
    [Migration(1)]
    public class M01_CreateSchema : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("AssignedAndSequentials")
                .WithColumn("AssignedId").AsInt32().PrimaryKey()
                .WithColumn("SequentialId").AsInt32().PrimaryKey()
                .WithColumn("Title").AsString().NotNullable();

            Create.Table("AssignedPairAndSequential")
                .WithColumn("FirstAssignedId").AsInt32().PrimaryKey()
                .WithColumn("SecondAssignedId").AsInt32().PrimaryKey()
                .WithColumn("SequentialId").AsInt32().PrimaryKey()
                .WithColumn("Title").AsString().NotNullable();

            Create.Table("Cities")
                .WithColumn("CityId").AsInt32().PrimaryKey().Identity()
                .WithColumn("CityCode").AsString().NotNullable()
                .WithColumn("Name").AsString().NotNullable()
                .WithColumn("Area").AsString().NotNullable();

            Create.Table("CitiesManual")
                .WithColumn("CityCode").AsString().PrimaryKey()
                .WithColumn("Name").AsString().NotNullable();

            Create.Table("CitiesSequential")
                .WithColumn("CityId").AsInt32().PrimaryKey()
                .WithColumn("CityCode").AsString().NotNullable()
                .WithColumn("Name").AsString().NotNullable();

            Create.Table("DateStampTest")
                .WithColumn("Name").AsString().PrimaryKey()
                .WithColumn("Value").AsString().NotNullable()
                .WithColumn("InsertDate").AsDateTime().NotNullable()
                .WithColumn("UpdateDate").AsDateTime().NotNullable();

            Create.Table("ReadOnlies")
                .WithColumn("SequentialId").AsInt16().PrimaryKey()
                .WithColumn("Editable").AsString()
                .WithColumn("ReadOnly").AsString().WithDefaultValue("Default");

            IfDatabase(ProcessorId.SQLite)
                .Create.Table("SoftDeleteTest")
                    .WithColumn("SoftDeleteId").AsInt32().PrimaryKey().Identity()
                    .WithColumn("Value").AsString().NotNullable()
                    .WithColumn("RecordStatus").AsInt32().NotNullable();

            IfDatabase(x => x != ProcessorId.SQLite).Create.Schema("Entatea");
            IfDatabase(x => x != ProcessorId.SQLite)
                .Create.Table("SoftDeleteTest")
                    .InSchema("Entatea")
                    .WithColumn("SoftDeleteId").AsInt32().PrimaryKey().Identity()
                    .WithColumn("Value").AsString().NotNullable()
                    .WithColumn("RecordStatus").AsInt32().NotNullable();
        }
    }
}
