using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class ChannelsFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "DMChannels");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "DMChannels");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "DMChannels");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "DMChannels");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "DMChannels");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "DMChannels");

            migrationBuilder.DropColumn(
                name: "ArchiveAfter",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "Archived",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "Locked",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "Members",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "Private",
                table: "Channels");

            migrationBuilder.CreateTable(
                name: "GroupDMChannels",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OwnerId = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Icon = table.Column<string>(type: "TEXT", nullable: true),
                    Members = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Type = table.Column<ushort>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupDMChannels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Threads",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<long>(type: "INTEGER", nullable: true),
                    ServerId = table.Column<long>(type: "INTEGER", nullable: false),
                    OwnerId = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Icon = table.Column<string>(type: "TEXT", nullable: true),
                    Members = table.Column<string>(type: "TEXT", nullable: false),
                    Position = table.Column<ushort>(type: "INTEGER", nullable: false),
                    Slowmode = table.Column<ulong>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastMessage = table.Column<long>(type: "INTEGER", nullable: false),
                    ArchiveAfter = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Archived = table.Column<bool>(type: "INTEGER", nullable: false),
                    Private = table.Column<bool>(type: "INTEGER", nullable: false),
                    Locked = table.Column<bool>(type: "INTEGER", nullable: false),
                    Type = table.Column<ushort>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Threads", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupDMChannels");

            migrationBuilder.DropTable(
                name: "Threads");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "DMChannels",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "DMChannels",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "DMChannels",
                type: "TEXT",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "DMChannels",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "DMChannels",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OwnerId",
                table: "DMChannels",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ulong>(
                name: "ArchiveAfter",
                table: "Channels",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Archived",
                table: "Channels",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Channels",
                type: "TEXT",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Locked",
                table: "Channels",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Members",
                table: "Channels",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OwnerId",
                table: "Channels",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Private",
                table: "Channels",
                type: "INTEGER",
                nullable: true);
        }
    }
}
