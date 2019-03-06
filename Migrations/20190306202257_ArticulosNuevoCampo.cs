using Microsoft.EntityFrameworkCore.Migrations;

namespace RestApiSigedi.Migrations
{
    public partial class ArticulosNuevoCampo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Codigo",
                table: "Articulos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "Articulos");
        }
    }
}
