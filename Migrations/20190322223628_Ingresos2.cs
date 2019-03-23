using Microsoft.EntityFrameworkCore.Migrations;

namespace RestApiSigedi.Migrations
{
    public partial class Ingresos2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IngresoDetalles_Articulos_IdArticulo",
                table: "IngresoDetalles");

            migrationBuilder.DropForeignKey(
                name: "FK_IngresoDetalles_Precios_IdPrecio",
                table: "IngresoDetalles");

            migrationBuilder.AddColumn<long>(
                name: "IdEdicion",
                table: "IngresoDetalles",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_IngresoDetalles_IdEdicion",
                table: "IngresoDetalles",
                column: "IdEdicion");

            migrationBuilder.AddForeignKey(
                name: "FK_IngresoDetalles_Articulos_IdArticulo",
                table: "IngresoDetalles",
                column: "IdArticulo",
                principalTable: "Articulos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IngresoDetalles_Ediciones_IdEdicion",
                table: "IngresoDetalles",
                column: "IdEdicion",
                principalTable: "Ediciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IngresoDetalles_Precios_IdPrecio",
                table: "IngresoDetalles",
                column: "IdPrecio",
                principalTable: "Precios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IngresoDetalles_Articulos_IdArticulo",
                table: "IngresoDetalles");

            migrationBuilder.DropForeignKey(
                name: "FK_IngresoDetalles_Ediciones_IdEdicion",
                table: "IngresoDetalles");

            migrationBuilder.DropForeignKey(
                name: "FK_IngresoDetalles_Precios_IdPrecio",
                table: "IngresoDetalles");

            migrationBuilder.DropIndex(
                name: "IX_IngresoDetalles_IdEdicion",
                table: "IngresoDetalles");

            migrationBuilder.DropColumn(
                name: "IdEdicion",
                table: "IngresoDetalles");

            migrationBuilder.AddForeignKey(
                name: "FK_IngresoDetalles_Articulos_IdArticulo",
                table: "IngresoDetalles",
                column: "IdArticulo",
                principalTable: "Articulos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IngresoDetalles_Precios_IdPrecio",
                table: "IngresoDetalles",
                column: "IdPrecio",
                principalTable: "Precios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
