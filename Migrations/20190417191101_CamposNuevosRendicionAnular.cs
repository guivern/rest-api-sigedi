using Microsoft.EntityFrameworkCore.Migrations;

namespace RestApiSigedi.Migrations
{
    public partial class CamposNuevosRendicionAnular : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Anulable",
                table: "Rendiciones",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Anulado",
                table: "Rendiciones",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Anulable",
                table: "RendicionDetalles",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Anulado",
                table: "RendicionDetalles",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Anulable",
                table: "Rendiciones");

            migrationBuilder.DropColumn(
                name: "Anulado",
                table: "Rendiciones");

            migrationBuilder.DropColumn(
                name: "Anulable",
                table: "RendicionDetalles");

            migrationBuilder.DropColumn(
                name: "Anulado",
                table: "RendicionDetalles");
        }
    }
}
