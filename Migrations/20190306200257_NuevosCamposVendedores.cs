using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RestApiSigedi.Migrations
{
    public partial class NuevosCamposVendedores : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaIngreso",
                table: "Vendedores",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nacionalidad",
                table: "Vendedores",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelefonoMovil",
                table: "Vendedores",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaIngreso",
                table: "Vendedores");

            migrationBuilder.DropColumn(
                name: "Nacionalidad",
                table: "Vendedores");

            migrationBuilder.DropColumn(
                name: "TelefonoMovil",
                table: "Vendedores");
        }
    }
}
