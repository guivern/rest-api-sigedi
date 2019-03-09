using Microsoft.EntityFrameworkCore.Migrations;

namespace RestApiSigedi.Migrations
{
    public partial class NuevosCamposProveedores : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Contacto",
                table: "Proveedores",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DireccionWeb",
                table: "Proveedores",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumeroContacto",
                table: "Proveedores",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Contacto",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "DireccionWeb",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "NumeroContacto",
                table: "Proveedores");
        }
    }
}
