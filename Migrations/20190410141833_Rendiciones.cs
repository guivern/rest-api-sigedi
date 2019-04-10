using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RestApiSigedi.Migrations
{
    public partial class Rendiciones : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rendiciones",
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
                    IdVendedor = table.Column<long>(nullable: false),
                    IdDistribucion = table.Column<long>(nullable: false),
                    MontoTotal = table.Column<decimal>(nullable: false),
                    ImporteTotal = table.Column<decimal>(nullable: false),
                    SaldoTotal = table.Column<decimal>(nullable: false),
                    TipoComprobante = table.Column<string>(nullable: true),
                    NroComprobante = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rendiciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rendiciones_Distribuciones_IdDistribucion",
                        column: x => x.IdDistribucion,
                        principalTable: "Distribuciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rendiciones_Usuarios_IdUsuarioCreador",
                        column: x => x.IdUsuarioCreador,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rendiciones_Usuarios_IdUsuarioModificador",
                        column: x => x.IdUsuarioModificador,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rendiciones_Vendedores_IdVendedor",
                        column: x => x.IdVendedor,
                        principalTable: "Vendedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RendicionDetalles",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Descripcion = table.Column<string>(nullable: true),
                    Activo = table.Column<bool>(nullable: false),
                    IdRendicion = table.Column<long>(nullable: false),
                    IdDistribucionDetalle = table.Column<long>(nullable: false),
                    Devoluciones = table.Column<long>(nullable: true),
                    Monto = table.Column<decimal>(nullable: false),
                    Importe = table.Column<decimal>(nullable: false),
                    Saldo = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RendicionDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RendicionDetalles_DistribucionDetalles_IdDistribucionDetalle",
                        column: x => x.IdDistribucionDetalle,
                        principalTable: "DistribucionDetalles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RendicionDetalles_Rendiciones_IdRendicion",
                        column: x => x.IdRendicion,
                        principalTable: "Rendiciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RendicionDetalles_IdDistribucionDetalle",
                table: "RendicionDetalles",
                column: "IdDistribucionDetalle");

            migrationBuilder.CreateIndex(
                name: "IX_RendicionDetalles_IdRendicion",
                table: "RendicionDetalles",
                column: "IdRendicion");

            migrationBuilder.CreateIndex(
                name: "IX_Rendiciones_IdDistribucion",
                table: "Rendiciones",
                column: "IdDistribucion");

            migrationBuilder.CreateIndex(
                name: "IX_Rendiciones_IdUsuarioCreador",
                table: "Rendiciones",
                column: "IdUsuarioCreador");

            migrationBuilder.CreateIndex(
                name: "IX_Rendiciones_IdUsuarioModificador",
                table: "Rendiciones",
                column: "IdUsuarioModificador");

            migrationBuilder.CreateIndex(
                name: "IX_Rendiciones_IdVendedor",
                table: "Rendiciones",
                column: "IdVendedor");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RendicionDetalles");

            migrationBuilder.DropTable(
                name: "Rendiciones");
        }
    }
}
