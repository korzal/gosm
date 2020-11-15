using Microsoft.EntityFrameworkCore.Migrations;

namespace GOSM.Migrations
{
    public partial class relevantgamesmanytomanyfix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RelevantGamesTable_GameGenreTable_GameGenreID",
                table: "RelevantGamesTable");

            migrationBuilder.DropForeignKey(
                name: "FK_RelevantGamesTable_UserTable_UserID",
                table: "RelevantGamesTable");

            migrationBuilder.DropTable(
                name: "GameGenreTable");

            migrationBuilder.DropIndex(
                name: "IX_RelevantGamesTable_GameGenreID",
                table: "RelevantGamesTable");

            migrationBuilder.DropIndex(
                name: "IX_RelevantGamesTable_UserID",
                table: "RelevantGamesTable");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "RelevantGamesTable");

            migrationBuilder.AddColumn<int>(
                name: "Genre",
                table: "RelevantGamesTable",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UserRelevantGamesTable",
                columns: table => new
                {
                    UserID = table.Column<int>(nullable: false),
                    RelevantGamesID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRelevantGamesTable", x => new { x.UserID, x.RelevantGamesID });
                    table.ForeignKey(
                        name: "FK_UserRelevantGamesTable_RelevantGamesTable_RelevantGamesID",
                        column: x => x.RelevantGamesID,
                        principalTable: "RelevantGamesTable",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRelevantGamesTable_UserTable_UserID",
                        column: x => x.UserID,
                        principalTable: "UserTable",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRelevantGamesTable_RelevantGamesID",
                table: "UserRelevantGamesTable",
                column: "RelevantGamesID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRelevantGamesTable");

            migrationBuilder.DropColumn(
                name: "Genre",
                table: "RelevantGamesTable");

            migrationBuilder.AddColumn<int>(
                name: "UserID",
                table: "RelevantGamesTable",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GameGenreTable",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameGenreTable", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RelevantGamesTable_GameGenreID",
                table: "RelevantGamesTable",
                column: "GameGenreID");

            migrationBuilder.CreateIndex(
                name: "IX_RelevantGamesTable_UserID",
                table: "RelevantGamesTable",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_RelevantGamesTable_GameGenreTable_GameGenreID",
                table: "RelevantGamesTable",
                column: "GameGenreID",
                principalTable: "GameGenreTable",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RelevantGamesTable_UserTable_UserID",
                table: "RelevantGamesTable",
                column: "UserID",
                principalTable: "UserTable",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
