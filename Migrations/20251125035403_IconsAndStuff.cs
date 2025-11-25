using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class IconsAndStuff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Servers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Channels",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentId",
                table: "Channels",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "Position",
                table: "Channels",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Servers");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "Channels");
        }
    }
}
