using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CnSharp.Data.UnitTest.Migrations
{
    public partial class RemoveRefreshCycle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SerialNumberRules",
                keyColumn: "Id",
                keyValue: "9ade83a8-4687-44b7-bac1-8d552ec7dcf6");

            migrationBuilder.DropColumn(
                name: "RefreshCycle",
                table: "SerialNumberRules");

            migrationBuilder.DropColumn(
                name: "D",
                table: "SerialNumberRollings");

            migrationBuilder.DropColumn(
                name: "M",
                table: "SerialNumberRollings");

            migrationBuilder.DropColumn(
                name: "Y",
                table: "SerialNumberRollings");

            migrationBuilder.AddColumn<string>(
                name: "Date",
                table: "SerialNumberRollings",
                nullable: true);

            migrationBuilder.InsertData(
                table: "SerialNumberRules",
                columns: new[] { "Id", "Code", "DateCreated", "Pattern", "StartValue", "Step" },
                values: new object[] { "8794f5dd-79ae-4e06-aa6c-d6a80bc78ea7", "PO", new DateTimeOffset(new DateTime(2024, 2, 25, 16, 27, 55, 912, DateTimeKind.Unspecified).AddTicks(3820), new TimeSpan(0, 8, 0, 0, 0)), "%wid%PO%yyyyMMdd%%seq5%", 1, 1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SerialNumberRules",
                keyColumn: "Id",
                keyValue: "8794f5dd-79ae-4e06-aa6c-d6a80bc78ea7");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "SerialNumberRollings");

            migrationBuilder.AddColumn<int>(
                name: "RefreshCycle",
                table: "SerialNumberRules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "D",
                table: "SerialNumberRollings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "M",
                table: "SerialNumberRollings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Y",
                table: "SerialNumberRollings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "SerialNumberRules",
                columns: new[] { "Id", "Code", "DateCreated", "Pattern", "RefreshCycle", "StartValue", "Step" },
                values: new object[] { "9ade83a8-4687-44b7-bac1-8d552ec7dcf6", "PO", new DateTimeOffset(new DateTime(2020, 2, 16, 10, 21, 50, 993, DateTimeKind.Unspecified).AddTicks(9694), new TimeSpan(0, 8, 0, 0, 0)), "%wid%PO%yyyyMMdd%%seq5%", 1, 1, 1 });
        }
    }
}
