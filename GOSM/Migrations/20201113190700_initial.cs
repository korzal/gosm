using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GOSM.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameGenreTable",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameGenreTable", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UserTable",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTable", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CommentTable",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentTable", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CommentTable_UserTable_UserID",
                        column: x => x.UserID,
                        principalTable: "UserTable",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FriendRequestTable",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderID = table.Column<int>(nullable: true),
                    RecipientID = table.Column<int>(nullable: true),
                    IsAccepted = table.Column<bool>(nullable: false),
                    RequestDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendRequestTable", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FriendRequestTable_UserTable_RecipientID",
                        column: x => x.RecipientID,
                        principalTable: "UserTable",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FriendRequestTable_UserTable_SenderID",
                        column: x => x.SenderID,
                        principalTable: "UserTable",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PostTable",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostTable", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PostTable_UserTable_UserID",
                        column: x => x.UserID,
                        principalTable: "UserTable",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RelevantGamesTable",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true),
                    GameGenreID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelevantGamesTable", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RelevantGamesTable_GameGenreTable_GameGenreID",
                        column: x => x.GameGenreID,
                        principalTable: "GameGenreTable",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RelevantGamesTable_UserTable_UserID",
                        column: x => x.UserID,
                        principalTable: "UserTable",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentTable_UserID",
                table: "CommentTable",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequestTable_RecipientID",
                table: "FriendRequestTable",
                column: "RecipientID");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequestTable_SenderID",
                table: "FriendRequestTable",
                column: "SenderID");

            migrationBuilder.CreateIndex(
                name: "IX_PostTable_UserID",
                table: "PostTable",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_RelevantGamesTable_GameGenreID",
                table: "RelevantGamesTable",
                column: "GameGenreID");

            migrationBuilder.CreateIndex(
                name: "IX_RelevantGamesTable_UserID",
                table: "RelevantGamesTable",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentTable");

            migrationBuilder.DropTable(
                name: "FriendRequestTable");

            migrationBuilder.DropTable(
                name: "PostTable");

            migrationBuilder.DropTable(
                name: "RelevantGamesTable");

            migrationBuilder.DropTable(
                name: "GameGenreTable");

            migrationBuilder.DropTable(
                name: "UserTable");
        }
    }
}
