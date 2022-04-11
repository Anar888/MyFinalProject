using Microsoft.EntityFrameworkCore.Migrations;

namespace MilanoFashion.Migrations
{
    public partial class TeamTableCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Position = table.Column<string>(nullable: true),
                    Image = table.Column<string>(nullable: true),
                    Fburl = table.Column<string>(nullable: true),
                    Twiturl = table.Column<string>(nullable: true),
                    Youtubeurl = table.Column<string>(nullable: true),
                    Googleurl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Teams");
        }
    }
}
