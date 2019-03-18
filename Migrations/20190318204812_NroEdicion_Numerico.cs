using Microsoft.EntityFrameworkCore.Migrations;

namespace RestApiSigedi.Migrations
{
    public partial class NroEdicion_Numerico : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE \"IngresoDetalles\" ALTER COLUMN \"NroEdicion\" TYPE BIGINT USING 0" 
            );

            migrationBuilder.Sql(
                "ALTER TABLE \"Ediciones\" ALTER COLUMN \"NroEdicion\" TYPE BIGINT USING 0" 
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NroEdicion",
                table: "IngresoDetalles",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<string>(
                name: "NroEdicion",
                table: "Ediciones",
                nullable: true,
                oldClrType: typeof(long));
        }
    }
}
