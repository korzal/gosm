using Microsoft.EntityFrameworkCore.Migrations;

namespace GOSM.Migrations
{
    public partial class assignToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JwtToken",
                table: "UserTable",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JwtToken",
                table: "UserTable");
        }
    }
}
