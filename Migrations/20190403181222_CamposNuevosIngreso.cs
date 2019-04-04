using Microsoft.EntityFrameworkCore.Migrations;

namespace RestApiSigedi.Migrations
{
    public partial class CamposNuevosIngreso : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Anulable",
                table: "Ingresos",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Anulado",
                table: "Ingresos",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Editable",
                table: "Ingresos",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Anulable",
                table: "IngresoDetalles",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Anulado",
                table: "IngresoDetalles",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Editable",
                table: "IngresoDetalles",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Anulable",
                table: "Ingresos");

            migrationBuilder.DropColumn(
                name: "Anulado",
                table: "Ingresos");

            migrationBuilder.DropColumn(
                name: "Editable",
                table: "Ingresos");

            migrationBuilder.DropColumn(
                name: "Anulable",
                table: "IngresoDetalles");

            migrationBuilder.DropColumn(
                name: "Anulado",
                table: "IngresoDetalles");

            migrationBuilder.DropColumn(
                name: "Editable",
                table: "IngresoDetalles");
        }
    }
}
