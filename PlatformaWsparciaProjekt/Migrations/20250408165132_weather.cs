using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlatformaWsparciaProjekt.Migrations
{
    /// <inheritdoc />
    public partial class weather : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "HelpRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "HelpRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "HelpRequests");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "HelpRequests");
        }
    }
}
