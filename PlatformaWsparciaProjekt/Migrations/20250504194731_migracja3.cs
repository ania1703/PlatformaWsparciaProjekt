using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlatformaWsparciaProjekt.Migrations
{
    /// <inheritdoc />
    public partial class migracja3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShoppingListId",
                table: "HelpRequests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HelpRequests_ShoppingListId",
                table: "HelpRequests",
                column: "ShoppingListId");

            migrationBuilder.AddForeignKey(
                name: "FK_HelpRequests_ShoppingLists_ShoppingListId",
                table: "HelpRequests",
                column: "ShoppingListId",
                principalTable: "ShoppingLists",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HelpRequests_ShoppingLists_ShoppingListId",
                table: "HelpRequests");

            migrationBuilder.DropIndex(
                name: "IX_HelpRequests_ShoppingListId",
                table: "HelpRequests");

            migrationBuilder.DropColumn(
                name: "ShoppingListId",
                table: "HelpRequests");
        }
    }
}
