using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RestApiSigedi.Migrations
{
    public partial class Cajas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "IdCaja",
                table: "Rendiciones",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "Cajas",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Descripcion = table.Column<string>(nullable: true),
                    Activo = table.Column<bool>(nullable: false),
                    IdUsuarioCreador = table.Column<long>(nullable: true),
                    IdUsuarioModificador = table.Column<long>(nullable: true),
                    FechaCreacion = table.Column<DateTime>(nullable: false),
                    FechaUltimaModificacion = table.Column<DateTime>(nullable: true),
                    FechaCierre = table.Column<DateTime>(nullable: true),
                    MontoInicial = table.Column<decimal>(nullable: true),
                    Monto = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cajas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cajas_Usuarios_IdUsuarioCreador",
                        column: x => x.IdUsuarioCreador,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cajas_Usuarios_IdUsuarioModificador",
                        column: x => x.IdUsuarioModificador,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rendiciones_IdCaja",
                table: "Rendiciones",
                column: "IdCaja");

            migrationBuilder.CreateIndex(
                name: "IX_Cajas_IdUsuarioCreador",
                table: "Cajas",
                column: "IdUsuarioCreador");

            migrationBuilder.CreateIndex(
                name: "IX_Cajas_IdUsuarioModificador",
                table: "Cajas",
                column: "IdUsuarioModificador");

            migrationBuilder.AddForeignKey(
                name: "FK_Rendiciones_Cajas_IdCaja",
                table: "Rendiciones",
                column: "IdCaja",
                principalTable: "Cajas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rendiciones_Cajas_IdCaja",
                table: "Rendiciones");

            migrationBuilder.DropTable(
                name: "Cajas");

            migrationBuilder.DropIndex(
                name: "IX_Rendiciones_IdCaja",
                table: "Rendiciones");

            migrationBuilder.DropColumn(
                name: "IdCaja",
                table: "Rendiciones");
        }
    }
}
