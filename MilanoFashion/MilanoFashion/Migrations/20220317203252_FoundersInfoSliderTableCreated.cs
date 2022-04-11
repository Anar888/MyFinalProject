using Microsoft.EntityFrameworkCore.Migrations;

namespace MilanoFashion.Migrations
{
    public partial class FoundersInfoSliderTableCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FoundersInfoSliders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 150, nullable: true),
                    Image = table.Column<string>(maxLength: 200, nullable: true),
                    Bio = table.Column<string>(maxLength: 500, nullable: true),
                    Position = table.Column<string>(maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoundersInfoSliders", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoundersInfoSliders");
        }
    }
}
