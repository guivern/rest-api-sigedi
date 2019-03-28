using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RestApiSigedi.Migrations
{
    public partial class Distribuciones : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Distribuciones",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Descripcion = table.Column<string>(nullable: true),
                    Activo = table.Column<bool>(nullable: false),
                    IdUsuarioCreador = table.Column<long>(nullable: false),
                    IdUsuarioModificador = table.Column<long>(nullable: true),
                    FechaCreacion = table.Column<DateTime>(nullable: false),
                    FechaUltimaModificacion = table.Column<DateTime>(nullable: true),
                    IdVendedor = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Distribuciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Distribuciones_Usuarios_IdUsuarioCreador",
                        column: x => x.IdUsuarioCreador,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Distribuciones_Usuarios_IdUsuarioModificador",
                        column: x => x.IdUsuarioModificador,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Distribuciones_Vendedores_IdVendedor",
                        column: x => x.IdVendedor,
                        principalTable: "Vendedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Movimientos",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Descripcion = table.Column<string>(nullable: true),
                    Activo = table.Column<bool>(nullable: false),
                    IdEdicion = table.Column<long>(nullable: false),
                    IdVendedor = table.Column<long>(nullable: false),
                    Llevo = table.Column<long>(nullable: false),
                    Devolvio = table.Column<long>(nullable: true),
                    Monto = table.Column<decimal>(nullable: false),
                    Importe = table.Column<decimal>(nullable: true),
                    Saldo = table.Column<decimal>(nullable: false),
                    Anulado = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movimientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Movimientos_Ediciones_IdEdicion",
                        column: x => x.IdEdicion,
                        principalTable: "Ediciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Movimientos_Vendedores_IdVendedor",
                        column: x => x.IdVendedor,
                        principalTable: "Vendedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DistribucionDetalles",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Descripcion = table.Column<string>(nullable: true),
                    IdDistribucion = table.Column<long>(nullable: false),
                    IdEdicion = table.Column<long>(nullable: false),
                    IdMovimiento = table.Column<long>(nullable: false),
                    Cantidad = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DistribucionDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DistribucionDetalles_Distribuciones_IdDistribucion",
                        column: x => x.IdDistribucion,
                        principalTable: "Distribuciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DistribucionDetalles_Ediciones_IdEdicion",
                        column: x => x.IdEdicion,
                        principalTable: "Ediciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DistribucionDetalles_Movimientos_IdMovimiento",
                        column: x => x.IdMovimiento,
                        principalTable: "Movimientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DistribucionDetalles_IdDistribucion",
                table: "DistribucionDetalles",
                column: "IdDistribucion");

            migrationBuilder.CreateIndex(
                name: "IX_DistribucionDetalles_IdEdicion",
                table: "DistribucionDetalles",
                column: "IdEdicion");

            migrationBuilder.CreateIndex(
                name: "IX_DistribucionDetalles_IdMovimiento",
                table: "DistribucionDetalles",
                column: "IdMovimiento");

            migrationBuilder.CreateIndex(
                name: "IX_Distribuciones_IdUsuarioCreador",
                table: "Distribuciones",
                column: "IdUsuarioCreador");

            migrationBuilder.CreateIndex(
                name: "IX_Distribuciones_IdUsuarioModificador",
                table: "Distribuciones",
                column: "IdUsuarioModificador");

            migrationBuilder.CreateIndex(
                name: "IX_Distribuciones_IdVendedor",
                table: "Distribuciones",
                column: "IdVendedor");

            migrationBuilder.CreateIndex(
                name: "IX_Movimientos_IdEdicion",
                table: "Movimientos",
                column: "IdEdicion");

            migrationBuilder.CreateIndex(
                name: "IX_Movimientos_IdVendedor",
                table: "Movimientos",
                column: "IdVendedor");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DistribucionDetalles");

            migrationBuilder.DropTable(
                name: "Distribuciones");

            migrationBuilder.DropTable(
                name: "Movimientos");
        }
    }
}
