using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MilanoFashion.Migrations
{
    public partial class OrderTableUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Orders",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Orders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Orders",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Orders");
        }
    }
}
