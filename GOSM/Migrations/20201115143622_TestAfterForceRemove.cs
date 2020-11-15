using Microsoft.EntityFrameworkCore.Migrations;

namespace GOSM.Migrations
{
    public partial class TestAfterForceRemove : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "UserTable",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "UserTable",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "UserTable",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "RelevantGamesTable",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PostID",
                table: "CommentTable",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommentTable_PostID",
                table: "CommentTable",
                column: "PostID");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentTable_PostTable_PostID",
                table: "CommentTable",
                column: "PostID",
                principalTable: "PostTable",
                principalColumn: "ID",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentTable_PostTable_PostID",
                table: "CommentTable");

            migrationBuilder.DropIndex(
                name: "IX_CommentTable_PostID",
                table: "CommentTable");

            migrationBuilder.DropColumn(
                name: "PostID",
                table: "CommentTable");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "UserTable",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "UserTable",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "UserTable",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "RelevantGamesTable",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
