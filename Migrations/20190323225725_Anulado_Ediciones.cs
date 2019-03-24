using Microsoft.EntityFrameworkCore.Migrations;

namespace RestApiSigedi.Migrations
{
    public partial class Anulado_Ediciones : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Anulado",
                table: "Ediciones",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Anulado",
                table: "Ediciones");
        }
    }
}
