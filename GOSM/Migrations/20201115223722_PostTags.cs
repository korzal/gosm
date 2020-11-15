using Microsoft.EntityFrameworkCore.Migrations;

namespace GOSM.Migrations
{
    public partial class PostTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentTable_PostTable_PostID",
                table: "CommentTable");

            migrationBuilder.DropForeignKey(
                name: "FK_FriendRequestTable_UserTable_RecipientID",
                table: "FriendRequestTable");

            migrationBuilder.DropForeignKey(
                name: "FK_FriendRequestTable_UserTable_SenderID",
                table: "FriendRequestTable");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "PostTable",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Tag",
                table: "PostTable",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "SenderID",
                table: "FriendRequestTable",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RecipientID",
                table: "FriendRequestTable",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "CommentTable",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PostID",
                table: "CommentTable",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentTable_PostTable_PostID",
                table: "CommentTable",
                column: "PostID",
                principalTable: "PostTable",
                principalColumn: "ID",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_FriendRequestTable_UserTable_RecipientID",
                table: "FriendRequestTable",
                column: "RecipientID",
                principalTable: "UserTable",
                principalColumn: "ID",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_FriendRequestTable_UserTable_SenderID",
                table: "FriendRequestTable",
                column: "SenderID",
                principalTable: "UserTable",
                principalColumn: "ID",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentTable_PostTable_PostID",
                table: "CommentTable");

            migrationBuilder.DropForeignKey(
                name: "FK_FriendRequestTable_UserTable_RecipientID",
                table: "FriendRequestTable");

            migrationBuilder.DropForeignKey(
                name: "FK_FriendRequestTable_UserTable_SenderID",
                table: "FriendRequestTable");

            migrationBuilder.DropColumn(
                name: "Tag",
                table: "PostTable");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "PostTable",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<int>(
                name: "SenderID",
                table: "FriendRequestTable",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "RecipientID",
                table: "FriendRequestTable",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "CommentTable",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<int>(
                name: "PostID",
                table: "CommentTable",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_CommentTable_PostTable_PostID",
                table: "CommentTable",
                column: "PostID",
                principalTable: "PostTable",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FriendRequestTable_UserTable_RecipientID",
                table: "FriendRequestTable",
                column: "RecipientID",
                principalTable: "UserTable",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FriendRequestTable_UserTable_SenderID",
                table: "FriendRequestTable",
                column: "SenderID",
                principalTable: "UserTable",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
