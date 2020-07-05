using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Allowed.Telegram.Bot.Data.Migrations
{
    public partial class RemoveUserRoleKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TelegramUserRoles",
                table: "TelegramUserRoles");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "TelegramUserRoles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "TelegramUserRoles",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TelegramUserRoles",
                table: "TelegramUserRoles",
                column: "Id");
        }
    }
}
