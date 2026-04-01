using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace tms_api.Data.Migrations
{
    public partial class UpdateDatabaseV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "Comment",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "FbUserTokens",
                columns: table => new
                {
                    FbUserTokenID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FbUserTokens", x => x.FbUserTokenID);
                    table.ForeignKey(
                        name: "FK_UserFbUserToken",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FbUserTokens_UserId",
                table: "FbUserTokens",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserComment",
                table: "Comment");

            migrationBuilder.DropTable(
                name: "FbUserTokens");

            migrationBuilder.DropIndex(
                name: "IX_Comment_UserID",
                table: "Comment");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "Comment",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
