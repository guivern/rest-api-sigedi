using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RestApiSigedi.Migrations
{
    public partial class TablaEgreso : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Egresos",
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
                    IdEdicion = table.Column<long>(nullable: false),
                    Cantidad = table.Column<long>(nullable: false),
                    Fecha = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Egresos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Egresos_Ediciones_IdEdicion",
                        column: x => x.IdEdicion,
                        principalTable: "Ediciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Egresos_Usuarios_IdUsuarioCreador",
                        column: x => x.IdUsuarioCreador,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Egresos_Usuarios_IdUsuarioModificador",
                        column: x => x.IdUsuarioModificador,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Egresos_IdEdicion",
                table: "Egresos",
                column: "IdEdicion");

            migrationBuilder.CreateIndex(
                name: "IX_Egresos_IdUsuarioCreador",
                table: "Egresos",
                column: "IdUsuarioCreador");

            migrationBuilder.CreateIndex(
                name: "IX_Egresos_IdUsuarioModificador",
                table: "Egresos",
                column: "IdUsuarioModificador");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Egresos");
        }
    }
}
