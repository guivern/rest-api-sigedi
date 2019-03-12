using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RestApiSigedi.Migrations
{
    public partial class Ingresos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ediciones",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Descripcion = table.Column<string>(nullable: true),
                    Activo = table.Column<bool>(nullable: false),
                    IdArticulo = table.Column<long>(nullable: false),
                    IdPrecio = table.Column<long>(nullable: false),
                    FechaEdicion = table.Column<DateTime>(nullable: true),
                    NroEdicion = table.Column<string>(nullable: true),
                    CantidadInicial = table.Column<long>(nullable: false),
                    CantidadActual = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ediciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ediciones_Articulos_IdArticulo",
                        column: x => x.IdArticulo,
                        principalTable: "Articulos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ediciones_Precios_IdPrecio",
                        column: x => x.IdPrecio,
                        principalTable: "Precios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ingresos",
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
                    IdProveedor = table.Column<long>(nullable: false),
                    TipoComprobante = table.Column<string>(nullable: true),
                    NumeroComprobante = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingresos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ingresos_Proveedores_IdProveedor",
                        column: x => x.IdProveedor,
                        principalTable: "Proveedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ingresos_Usuarios_IdUsuarioCreador",
                        column: x => x.IdUsuarioCreador,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ingresos_Usuarios_IdUsuarioModificador",
                        column: x => x.IdUsuarioModificador,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IngresoDetalles",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Descripcion = table.Column<string>(nullable: true),
                    IdIngreso = table.Column<long>(nullable: false),
                    IdArticulo = table.Column<long>(nullable: false),
                    IdPrecio = table.Column<long>(nullable: false),
                    Cantidad = table.Column<long>(nullable: false),
                    FechaEdicion = table.Column<DateTime>(nullable: true),
                    NroEdicion = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngresoDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IngresoDetalles_Articulos_IdArticulo",
                        column: x => x.IdArticulo,
                        principalTable: "Articulos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IngresoDetalles_Ingresos_IdIngreso",
                        column: x => x.IdIngreso,
                        principalTable: "Ingresos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IngresoDetalles_Precios_IdPrecio",
                        column: x => x.IdPrecio,
                        principalTable: "Precios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ediciones_IdArticulo",
                table: "Ediciones",
                column: "IdArticulo");

            migrationBuilder.CreateIndex(
                name: "IX_Ediciones_IdPrecio",
                table: "Ediciones",
                column: "IdPrecio");

            migrationBuilder.CreateIndex(
                name: "IX_IngresoDetalles_IdArticulo",
                table: "IngresoDetalles",
                column: "IdArticulo");

            migrationBuilder.CreateIndex(
                name: "IX_IngresoDetalles_IdIngreso",
                table: "IngresoDetalles",
                column: "IdIngreso");

            migrationBuilder.CreateIndex(
                name: "IX_IngresoDetalles_IdPrecio",
                table: "IngresoDetalles",
                column: "IdPrecio");

            migrationBuilder.CreateIndex(
                name: "IX_Ingresos_IdProveedor",
                table: "Ingresos",
                column: "IdProveedor");

            migrationBuilder.CreateIndex(
                name: "IX_Ingresos_IdUsuarioCreador",
                table: "Ingresos",
                column: "IdUsuarioCreador");

            migrationBuilder.CreateIndex(
                name: "IX_Ingresos_IdUsuarioModificador",
                table: "Ingresos",
                column: "IdUsuarioModificador");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ediciones");

            migrationBuilder.DropTable(
                name: "IngresoDetalles");

            migrationBuilder.DropTable(
                name: "Ingresos");
        }
    }
}
