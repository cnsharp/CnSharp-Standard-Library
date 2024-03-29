﻿using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CnSharp.Data.SerialNumber.MySql.FunctionalTests.Migrations
{
    public partial class InitMySql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SerialNumberRolling",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Code = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    Date = table.Column<string>(type: "char(10)", nullable: false),
                    CurrentValue = table.Column<long>(nullable: false),
                    Version = table.Column<DateTime>(rowVersion: true, nullable: true)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    DateCreated = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)"),
                    DateUpdated = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SerialNumberRolling", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SerialNumberRule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Code = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    StartValue = table.Column<int>(nullable: false),
                    Step = table.Column<int>(nullable: false),
                    SequencePattern = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true),
                    NumberPattern = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)"),
                    DateUpdated = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SerialNumberRule", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "SerialNumberRule",
                columns: new[] { "Id", "Code", "DateCreated", "NumberPattern", "SequencePattern", "StartValue", "Step" },
                values: new object[] { new Guid("a99f370d-b4a9-4e27-8004-6cb6cf8bf89a"), "PO", new DateTimeOffset(new DateTime(2024, 3, 3, 0, 0, 6, 132, DateTimeKind.Unspecified).AddTicks(400), new TimeSpan(0, 8, 0, 0, 0)), "%wid%PO%yyyyMMdd%%06d%", "%wid%PO", 1, 1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SerialNumberRolling");

            migrationBuilder.DropTable(
                name: "SerialNumberRule");
        }
    }
}
