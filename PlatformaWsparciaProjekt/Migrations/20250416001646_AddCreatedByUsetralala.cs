using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlatformaWsparciaProjekt.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByUsetralala : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HelpRequests_Seniors_SeniorId",
                table: "HelpRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_HelpRequests_Volunteers_VolunteerId",
                table: "HelpRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_HelpRequests_Seniors_SeniorId",
                table: "HelpRequests",
                column: "SeniorId",
                principalTable: "Seniors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HelpRequests_Volunteers_VolunteerId",
                table: "HelpRequests",
                column: "VolunteerId",
                principalTable: "Volunteers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HelpRequests_Seniors_SeniorId",
                table: "HelpRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_HelpRequests_Volunteers_VolunteerId",
                table: "HelpRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_HelpRequests_Seniors_SeniorId",
                table: "HelpRequests",
                column: "SeniorId",
                principalTable: "Seniors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HelpRequests_Volunteers_VolunteerId",
                table: "HelpRequests",
                column: "VolunteerId",
                principalTable: "Volunteers",
                principalColumn: "Id");
        }
    }
}
