using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlatformaWsparciaProjekt.Migrations
{
    /// <inheritdoc />
    public partial class listaZakupow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFinalized",
                table: "ShoppingLists",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFinalized",
                table: "ShoppingLists");
        }
    }
}
