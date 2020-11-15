using Microsoft.EntityFrameworkCore.Migrations;

namespace GOSM.Migrations
{
    public partial class RelevantGamesenums : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameGenreID",
                table: "RelevantGamesTable");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GameGenreID",
                table: "RelevantGamesTable",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
