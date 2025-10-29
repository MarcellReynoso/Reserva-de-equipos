using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reserva_de_equipos.Migrations
{
    /// <inheritdoc />
    public partial class Responsable_Rol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RolId",
                schema: "Reserva",
                table: "Responsable",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Responsable_RolId",
                schema: "Reserva",
                table: "Responsable",
                column: "RolId");

            migrationBuilder.AddForeignKey(
                name: "FK_Responsable_Rol_RolId",
                schema: "Reserva",
                table: "Responsable",
                column: "RolId",
                principalSchema: "Reserva",
                principalTable: "Rol",
                principalColumn: "RolId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Responsable_Rol_RolId",
                schema: "Reserva",
                table: "Responsable");

            migrationBuilder.DropIndex(
                name: "IX_Responsable_RolId",
                schema: "Reserva",
                table: "Responsable");

            migrationBuilder.DropColumn(
                name: "RolId",
                schema: "Reserva",
                table: "Responsable");
        }
    }
}
