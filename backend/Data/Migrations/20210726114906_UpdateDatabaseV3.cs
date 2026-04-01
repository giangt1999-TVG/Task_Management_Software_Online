using Microsoft.EntityFrameworkCore.Migrations;

namespace tms_api.Data.Migrations
{
    public partial class UpdateDatabaseV3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RollNumber",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RollNumber",
                table: "Users");
        }
    }
}
