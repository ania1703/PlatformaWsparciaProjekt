using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlatformaWsparciaProjekt.Migrations
{
    /// <inheritdoc />
    public partial class RequestHelp3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VolunteerId",
                table: "HelpRequests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HelpRequests_VolunteerId",
                table: "HelpRequests",
                column: "VolunteerId");

            migrationBuilder.AddForeignKey(
                name: "FK_HelpRequests_Volunteers_VolunteerId",
                table: "HelpRequests",
                column: "VolunteerId",
                principalTable: "Volunteers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HelpRequests_Volunteers_VolunteerId",
                table: "HelpRequests");

            migrationBuilder.DropIndex(
                name: "IX_HelpRequests_VolunteerId",
                table: "HelpRequests");

            migrationBuilder.DropColumn(
                name: "VolunteerId",
                table: "HelpRequests");
        }
    }
}
