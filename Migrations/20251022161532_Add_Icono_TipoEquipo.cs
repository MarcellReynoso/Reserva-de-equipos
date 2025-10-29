using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reserva_de_equipos.Migrations
{
    /// <inheritdoc />
    public partial class Add_Icono_TipoEquipo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IconoUrl",
                schema: "Reserva",
                table: "Tipo_Equipo",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IconoUrl",
                schema: "Reserva",
                table: "Tipo_Equipo");
        }

    }
}
