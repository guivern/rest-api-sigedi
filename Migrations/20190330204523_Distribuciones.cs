using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RestApiSigedi.Migrations
{
    public partial class Distribuciones : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Egresos_Usuarios_IdUsuarioCreador",
                table: "Egresos");

            migrationBuilder.DropForeignKey(
                name: "FK_Ingresos_Usuarios_IdUsuarioCreador",
                table: "Ingresos");

            migrationBuilder.AlterColumn<long>(
                name: "IdUsuarioCreador",
                table: "Ingresos",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<long>(
                name: "IdUsuarioCreador",
                table: "Egresos",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Articulos",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaUltimaModificacion",
                table: "Articulos",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "IdUsuarioCreador",
                table: "Articulos",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "IdUsuarioModificador",
                table: "Articulos",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Distribuciones",
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
                        onDelete: ReferentialAction.Restrict);
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
                name: "DistribucionDetalles",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Descripcion = table.Column<string>(nullable: true),
                    Activo = table.Column<bool>(nullable: false),
                    IdDistribucion = table.Column<long>(nullable: false),
                    IdEdicion = table.Column<long>(nullable: false),
                    Cantidad = table.Column<long>(nullable: false),
                    Devoluciones = table.Column<long>(nullable: true),
                    Monto = table.Column<decimal>(nullable: false),
                    Importe = table.Column<decimal>(nullable: true),
                    Saldo = table.Column<decimal>(nullable: false),
                    Anulable = table.Column<bool>(nullable: true),
                    Editable = table.Column<bool>(nullable: true)
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
                });

            migrationBuilder.CreateIndex(
                name: "IX_Articulos_IdUsuarioCreador",
                table: "Articulos",
                column: "IdUsuarioCreador");

            migrationBuilder.CreateIndex(
                name: "IX_Articulos_IdUsuarioModificador",
                table: "Articulos",
                column: "IdUsuarioModificador");

            migrationBuilder.CreateIndex(
                name: "IX_DistribucionDetalles_IdDistribucion",
                table: "DistribucionDetalles",
                column: "IdDistribucion");

            migrationBuilder.CreateIndex(
                name: "IX_DistribucionDetalles_IdEdicion",
                table: "DistribucionDetalles",
                column: "IdEdicion");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Articulos_Usuarios_IdUsuarioCreador",
                table: "Articulos",
                column: "IdUsuarioCreador",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Articulos_Usuarios_IdUsuarioModificador",
                table: "Articulos",
                column: "IdUsuarioModificador",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Egresos_Usuarios_IdUsuarioCreador",
                table: "Egresos",
                column: "IdUsuarioCreador",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ingresos_Usuarios_IdUsuarioCreador",
                table: "Ingresos",
                column: "IdUsuarioCreador",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articulos_Usuarios_IdUsuarioCreador",
                table: "Articulos");

            migrationBuilder.DropForeignKey(
                name: "FK_Articulos_Usuarios_IdUsuarioModificador",
                table: "Articulos");

            migrationBuilder.DropForeignKey(
                name: "FK_Egresos_Usuarios_IdUsuarioCreador",
                table: "Egresos");

            migrationBuilder.DropForeignKey(
                name: "FK_Ingresos_Usuarios_IdUsuarioCreador",
                table: "Ingresos");

            migrationBuilder.DropTable(
                name: "DistribucionDetalles");

            migrationBuilder.DropTable(
                name: "Distribuciones");

            migrationBuilder.DropIndex(
                name: "IX_Articulos_IdUsuarioCreador",
                table: "Articulos");

            migrationBuilder.DropIndex(
                name: "IX_Articulos_IdUsuarioModificador",
                table: "Articulos");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "Articulos");

            migrationBuilder.DropColumn(
                name: "FechaUltimaModificacion",
                table: "Articulos");

            migrationBuilder.DropColumn(
                name: "IdUsuarioCreador",
                table: "Articulos");

            migrationBuilder.DropColumn(
                name: "IdUsuarioModificador",
                table: "Articulos");

            migrationBuilder.AlterColumn<long>(
                name: "IdUsuarioCreador",
                table: "Ingresos",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "IdUsuarioCreador",
                table: "Egresos",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Egresos_Usuarios_IdUsuarioCreador",
                table: "Egresos",
                column: "IdUsuarioCreador",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ingresos_Usuarios_IdUsuarioCreador",
                table: "Ingresos",
                column: "IdUsuarioCreador",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
