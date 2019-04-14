using Microsoft.EntityFrameworkCore.Migrations;

namespace RestApiSigedi.Migrations
{
    public partial class CampoNuevoDistribucionDetalle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "YaSeDevolvio",
                table: "DistribucionDetalles",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YaSeDevolvio",
                table: "DistribucionDetalles");
        }
    }
}
