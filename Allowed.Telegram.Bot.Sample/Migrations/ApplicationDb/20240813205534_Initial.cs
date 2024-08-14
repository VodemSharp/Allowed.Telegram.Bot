using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Allowed.Telegram.Bot.Sample.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "telegram_bots",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_telegram_bots", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "telegram_roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_telegram_roles", x => x.id);
                    table.UniqueConstraint("ak_telegram_roles_name", x => x.name);
                });

            migrationBuilder.CreateTable(
                name: "telegram_users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    is_bot = table.Column<bool>(type: "boolean", nullable: false),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: true),
                    username = table.Column<string>(type: "text", nullable: true),
                    language_code = table.Column<string>(type: "text", nullable: true),
                    is_premium = table.Column<bool>(type: "boolean", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_telegram_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "telegram_bot_users",
                columns: table => new
                {
                    telegram_user_id = table.Column<long>(type: "bigint", nullable: false),
                    telegram_bot_id = table.Column<long>(type: "bigint", nullable: false),
                    blocked = table.Column<bool>(type: "boolean", nullable: false),
                    state = table.Column<string>(type: "text", nullable: true),
                    added_to_attachment_menu = table.Column<bool>(type: "boolean", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_telegram_bot_users", x => new { x.telegram_bot_id, x.telegram_user_id });
                    table.ForeignKey(
                        name: "fk_telegram_bot_users_telegram_bots_telegram_bot_id",
                        column: x => x.telegram_bot_id,
                        principalTable: "telegram_bots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_telegram_bot_users_telegram_users_telegram_user_id",
                        column: x => x.telegram_user_id,
                        principalTable: "telegram_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "telegram_bot_user_roles",
                columns: table => new
                {
                    telegram_bot_id = table.Column<long>(type: "bigint", nullable: false),
                    telegram_user_id = table.Column<long>(type: "bigint", nullable: false),
                    telegram_role_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_telegram_bot_user_roles", x => new { x.telegram_bot_id, x.telegram_user_id, x.telegram_role_id });
                    table.ForeignKey(
                        name: "fk_telegram_bot_user_roles_telegram_bot_users_telegram_bot_id_",
                        columns: x => new { x.telegram_bot_id, x.telegram_user_id },
                        principalTable: "telegram_bot_users",
                        principalColumns: new[] { "telegram_bot_id", "telegram_user_id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_telegram_bot_user_roles_telegram_roles_telegram_role_id",
                        column: x => x.telegram_role_id,
                        principalTable: "telegram_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_files",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    telegram_bot_id = table.Column<long>(type: "bigint", nullable: false),
                    telegram_user_id = table.Column<long>(type: "bigint", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_files", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_files_telegram_bot_users_telegram_bot_id_telegram_user",
                        columns: x => new { x.telegram_bot_id, x.telegram_user_id },
                        principalTable: "telegram_bot_users",
                        principalColumns: new[] { "telegram_bot_id", "telegram_user_id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_telegram_bot_user_roles_telegram_role_id",
                table: "telegram_bot_user_roles",
                column: "telegram_role_id");

            migrationBuilder.CreateIndex(
                name: "ix_telegram_bot_users_telegram_user_id",
                table: "telegram_bot_users",
                column: "telegram_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_telegram_users_username",
                table: "telegram_users",
                column: "username");

            migrationBuilder.CreateIndex(
                name: "ix_user_files_telegram_bot_id_telegram_user_id",
                table: "user_files",
                columns: new[] { "telegram_bot_id", "telegram_user_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "telegram_bot_user_roles");

            migrationBuilder.DropTable(
                name: "user_files");

            migrationBuilder.DropTable(
                name: "telegram_roles");

            migrationBuilder.DropTable(
                name: "telegram_bot_users");

            migrationBuilder.DropTable(
                name: "telegram_bots");

            migrationBuilder.DropTable(
                name: "telegram_users");
        }
    }
}
