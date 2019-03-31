using Microsoft.EntityFrameworkCore.Migrations;

namespace RestApiSigedi.Migrations
{
    public partial class Distribuciones2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Anulable",
                table: "Distribuciones",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Editable",
                table: "Distribuciones",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Anulable",
                table: "Distribuciones");

            migrationBuilder.DropColumn(
                name: "Editable",
                table: "Distribuciones");
        }
    }
}
