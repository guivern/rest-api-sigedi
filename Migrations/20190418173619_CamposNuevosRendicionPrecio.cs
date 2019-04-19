using Microsoft.EntityFrameworkCore.Migrations;

namespace RestApiSigedi.Migrations
{
    public partial class CamposNuevosRendicionPrecio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PrecioRendicion",
                table: "RendicionDetalles",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioVenta",
                table: "RendicionDetalles",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioRendicion",
                table: "DistribucionDetalles",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioVenta",
                table: "DistribucionDetalles",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrecioRendicion",
                table: "RendicionDetalles");

            migrationBuilder.DropColumn(
                name: "PrecioVenta",
                table: "RendicionDetalles");

            migrationBuilder.DropColumn(
                name: "PrecioRendicion",
                table: "DistribucionDetalles");

            migrationBuilder.DropColumn(
                name: "PrecioVenta",
                table: "DistribucionDetalles");
        }
    }
}
