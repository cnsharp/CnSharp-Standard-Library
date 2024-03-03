using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CnSharp.Data.SerialNumber.SqlServer.FunctionalTests.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SerialNumberRolling",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "NEWID()"),
                    Code = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true),
                    Date = table.Column<string>(type: "char(10)", nullable: true),
                    CurrentValue = table.Column<long>(nullable: false),
                    Version = table.Column<byte[]>(rowVersion: true, nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    DateUpdated = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SerialNumberRolling", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SerialNumberRule",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "NEWID()"),
                    Code = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true),
                    StartValue = table.Column<int>(nullable: false),
                    Step = table.Column<int>(nullable: false),
                    SequencePattern = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true),
                    NumberPattern = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    DateUpdated = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SerialNumberRule", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "SerialNumberRule",
                columns: new[] { "Id", "Code", "DateCreated", "NumberPattern", "SequencePattern", "StartValue", "Step" },
                values: new object[] { new Guid("6fd7bf8a-ba9f-4f92-8605-9f692c698e3e"), "PO", new DateTimeOffset(new DateTime(2024, 2, 25, 22, 21, 13, 947, DateTimeKind.Unspecified).AddTicks(5651), new TimeSpan(0, 8, 0, 0, 0)), "%wid%PO%yyyyMMdd%%06d%", "%wid%PO", 1, 1 });
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
