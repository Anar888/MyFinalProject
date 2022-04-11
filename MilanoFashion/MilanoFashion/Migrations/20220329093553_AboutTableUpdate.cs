using Microsoft.EntityFrameworkCore.Migrations;

namespace MilanoFashion.Migrations
{
    public partial class AboutTableUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Desc",
                table: "Abouts");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Abouts");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Abouts");

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "Abouts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "Abouts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Key",
                table: "Abouts");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "Abouts");

            migrationBuilder.AddColumn<string>(
                name: "Desc",
                table: "Abouts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Abouts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Abouts",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
