﻿using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Allowed.Telegram.Bot.Sample.Migrations
{
    public partial class InitDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TelegramBots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramBots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TelegramRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TelegramUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TelegramId = table.Column<long>(type: "bigint", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    LanguageCode = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TelegramBotUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TelegramUserId = table.Column<int>(type: "integer", nullable: false),
                    TelegramBotId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramBotUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TelegramBotUsers_TelegramBots_TelegramBotId",
                        column: x => x.TelegramBotId,
                        principalTable: "TelegramBots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TelegramBotUsers_TelegramUsers_TelegramUserId",
                        column: x => x.TelegramUserId,
                        principalTable: "TelegramUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TelegramUserId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFiles_TelegramUsers_TelegramUserId",
                        column: x => x.TelegramUserId,
                        principalTable: "TelegramUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TelegramBotUserRoles",
                columns: table => new
                {
                    TelegramBotUserId = table.Column<int>(type: "integer", nullable: false),
                    TelegramRoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramBotUserRoles", x => new { x.TelegramBotUserId, x.TelegramRoleId });
                    table.ForeignKey(
                        name: "FK_TelegramBotUserRoles_TelegramBotUsers_TelegramBotUserId",
                        column: x => x.TelegramBotUserId,
                        principalTable: "TelegramBotUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TelegramBotUserRoles_TelegramRoles_TelegramRoleId",
                        column: x => x.TelegramRoleId,
                        principalTable: "TelegramRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TelegramStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TelegramBotUserId = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TelegramStates_TelegramBotUsers_TelegramBotUserId",
                        column: x => x.TelegramBotUserId,
                        principalTable: "TelegramBotUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TelegramBotUserRoles_TelegramRoleId",
                table: "TelegramBotUserRoles",
                column: "TelegramRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TelegramBotUsers_TelegramBotId",
                table: "TelegramBotUsers",
                column: "TelegramBotId");

            migrationBuilder.CreateIndex(
                name: "IX_TelegramBotUsers_TelegramUserId",
                table: "TelegramBotUsers",
                column: "TelegramUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TelegramStates_TelegramBotUserId",
                table: "TelegramStates",
                column: "TelegramBotUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TelegramUsers_TelegramId",
                table: "TelegramUsers",
                column: "TelegramId");

            migrationBuilder.CreateIndex(
                name: "IX_TelegramUsers_Username",
                table: "TelegramUsers",
                column: "Username");

            migrationBuilder.CreateIndex(
                name: "IX_UserFiles_TelegramUserId",
                table: "UserFiles",
                column: "TelegramUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TelegramBotUserRoles");

            migrationBuilder.DropTable(
                name: "TelegramStates");

            migrationBuilder.DropTable(
                name: "UserFiles");

            migrationBuilder.DropTable(
                name: "TelegramRoles");

            migrationBuilder.DropTable(
                name: "TelegramBotUsers");

            migrationBuilder.DropTable(
                name: "TelegramBots");

            migrationBuilder.DropTable(
                name: "TelegramUsers");
        }
    }
}
