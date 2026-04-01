using Microsoft.EntityFrameworkCore.Migrations;

namespace tms_api.Data.Migrations
{
    public partial class UpdateDatabaseV4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RollNumber",
                table: "Users",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RollNumber",
                table: "Users",
                column: "RollNumber",
                unique: true,
                filter: "[RollNumber] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_RollNumber",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "RollNumber",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
