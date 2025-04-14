using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlatformaWsparciaProjekt.Migrations
{
    /// <inheritdoc />
    public partial class logowanieproba1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Volunteers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Volunteers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "Volunteers",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Seniors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Seniors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Seniors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Seniors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "Seniors",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Volunteers");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Volunteers");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Volunteers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Seniors");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Seniors");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Seniors");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Seniors");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Seniors");
        }
    }
}
