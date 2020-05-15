using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Allowed.Telegram.Bot.Data.Migrations
{
    public partial class AddRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TelegramRoles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TelegramUserRoles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TelegramUserId = table.Column<int>(nullable: false),
                    TelegramRoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramUserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TelegramUserRoles_TelegramRoles_TelegramRoleId",
                        column: x => x.TelegramRoleId,
                        principalTable: "TelegramRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TelegramUserRoles_TelegramUsers_TelegramUserId",
                        column: x => x.TelegramUserId,
                        principalTable: "TelegramUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TelegramUserRoles_TelegramRoleId",
                table: "TelegramUserRoles",
                column: "TelegramRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TelegramUserRoles_TelegramUserId",
                table: "TelegramUserRoles",
                column: "TelegramUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TelegramUserRoles");

            migrationBuilder.DropTable(
                name: "TelegramRoles");
        }
    }
}
