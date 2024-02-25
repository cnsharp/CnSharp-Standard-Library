using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CnSharp.Data.UnitTest.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "PO");

            migrationBuilder.CreateTable(
                name: "SerialNumberRollings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(36)", nullable: false),
                    Code = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true),
                    Y = table.Column<int>(nullable: false),
                    M = table.Column<int>(nullable: false),
                    D = table.Column<int>(nullable: false),
                    CurrentValue = table.Column<long>(nullable: false),
                    Version = table.Column<byte[]>(rowVersion: true, nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    DateUpdated = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SerialNumberRollings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SerialNumberRules",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(36)", nullable: false),
                    Code = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true),
                    StartValue = table.Column<int>(nullable: false),
                    Step = table.Column<int>(nullable: false),
                    Pattern = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true),
                    RefreshCycle = table.Column<int>(nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    DateUpdated = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SerialNumberRules", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "SerialNumberRules",
                columns: new[] { "Id", "Code", "DateCreated", "Pattern", "RefreshCycle", "StartValue", "Step" },
                values: new object[] { "9ade83a8-4687-44b7-bac1-8d552ec7dcf6", "PO", new DateTimeOffset(new DateTime(2020, 2, 16, 10, 21, 50, 993, DateTimeKind.Unspecified).AddTicks(9694), new TimeSpan(0, 8, 0, 0, 0)), "%wid%PO%yyyyMMdd%%seq5%", 1, 1, 1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SerialNumberRollings");

            migrationBuilder.DropTable(
                name: "SerialNumberRules");

            migrationBuilder.DropSequence(
                name: "PO");
        }
    }
}
