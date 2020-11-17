using Microsoft.EntityFrameworkCore.Migrations;

namespace Gamezzz.Migrations
{
    public partial class gameUpdate3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "photoName",
                table: "Games",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "photoName",
                table: "Games");
        }
    }
}
