﻿// <auto-generated />
using Allowed.Telegram.Bot.Sample.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Allowed.Telegram.Bot.Sample.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("Allowed.Telegram.Bot.Sample.DbModels.Allowed.ApplicationTgBot", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("TelegramBots");
                });

            modelBuilder.Entity("Allowed.Telegram.Bot.Sample.DbModels.Allowed.ApplicationTgBotUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("TelegramBotId")
                        .HasColumnType("int");

                    b.Property<int>("TelegramUserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TelegramBotId");

                    b.HasIndex("TelegramUserId");

                    b.ToTable("TelegramBotUsers");
                });

            modelBuilder.Entity("Allowed.Telegram.Bot.Sample.DbModels.Allowed.ApplicationTgBotUserRole", b =>
                {
                    b.Property<int>("TelegramBotUserId")
                        .HasColumnType("int");

                    b.Property<int>("TelegramRoleId")
                        .HasColumnType("int");

                    b.HasKey("TelegramBotUserId", "TelegramRoleId");

                    b.HasIndex("TelegramRoleId");

                    b.ToTable("TelegramBotUserRoles");
                });

            modelBuilder.Entity("Allowed.Telegram.Bot.Sample.DbModels.Allowed.ApplicationTgRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("TelegramRoles");
                });

            modelBuilder.Entity("Allowed.Telegram.Bot.Sample.DbModels.Allowed.ApplicationTgState", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("TelegramBotUserId")
                        .HasColumnType("int");

                    b.Property<string>("Value")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("TelegramBotUserId");

                    b.ToTable("TelegramStates");
                });

            modelBuilder.Entity("Allowed.Telegram.Bot.Sample.DbModels.Allowed.ApplicationTgUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("FirstName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("LanguageCode")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("LastName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<long>("TelegramId")
                        .HasColumnType("bigint");

                    b.Property<string>("Username")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("TelegramId");

                    b.HasIndex("Username");

                    b.ToTable("TelegramUsers");
                });

            modelBuilder.Entity("Allowed.Telegram.Bot.Sample.DbModels.UserFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("TelegramUserId")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Value")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("TelegramUserId");

                    b.ToTable("UserFiles");
                });

            modelBuilder.Entity("Allowed.Telegram.Bot.Sample.DbModels.Allowed.ApplicationTgBotUser", b =>
                {
                    b.HasOne("Allowed.Telegram.Bot.Sample.DbModels.Allowed.ApplicationTgBot", null)
                        .WithMany()
                        .HasForeignKey("TelegramBotId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Allowed.Telegram.Bot.Sample.DbModels.Allowed.ApplicationTgUser", null)
                        .WithMany()
                        .HasForeignKey("TelegramUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Allowed.Telegram.Bot.Sample.DbModels.Allowed.ApplicationTgBotUserRole", b =>
                {
                    b.HasOne("Allowed.Telegram.Bot.Sample.DbModels.Allowed.ApplicationTgBotUser", null)
                        .WithMany()
                        .HasForeignKey("TelegramBotUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Allowed.Telegram.Bot.Sample.DbModels.Allowed.ApplicationTgRole", null)
                        .WithMany()
                        .HasForeignKey("TelegramRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Allowed.Telegram.Bot.Sample.DbModels.Allowed.ApplicationTgState", b =>
                {
                    b.HasOne("Allowed.Telegram.Bot.Sample.DbModels.Allowed.ApplicationTgBotUser", null)
                        .WithMany()
                        .HasForeignKey("TelegramBotUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Allowed.Telegram.Bot.Sample.DbModels.UserFile", b =>
                {
                    b.HasOne("Allowed.Telegram.Bot.Sample.DbModels.Allowed.ApplicationTgUser", "TelegramUser")
                        .WithMany("UserFiles")
                        .HasForeignKey("TelegramUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TelegramUser");
                });

            modelBuilder.Entity("Allowed.Telegram.Bot.Sample.DbModels.Allowed.ApplicationTgUser", b =>
                {
                    b.Navigation("UserFiles");
                });
#pragma warning restore 612, 618
        }
    }
}