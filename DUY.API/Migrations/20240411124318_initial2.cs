using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DUY.API.Migrations
{
    public partial class initial2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Google",
                table: "customer",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "facebookId",
                table: "customer",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Google",
                table: "customer");

            migrationBuilder.DropColumn(
                name: "facebookId",
                table: "customer");
        }
    }
}
