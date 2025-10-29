using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reserva_de_equipos.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Reserva");

            migrationBuilder.CreateTable(
                name: "Conductor",
                schema: "Reserva",
                columns: table => new
                {
                    ConductorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    SegundoNombre = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    ApellidoPaterno = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    ApellidoMaterno = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    Disponible = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conductor", x => x.ConductorId);
                });

            migrationBuilder.CreateTable(
                name: "Empresa",
                schema: "Reserva",
                columns: table => new
                {
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Descripción = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresa", x => x.EmpresaId);
                });

            migrationBuilder.CreateTable(
                name: "Responsable",
                schema: "Reserva",
                columns: table => new
                {
                    ResponsableId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    SegundoNombre = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    ApellidoPaterno = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    ApellidoMaterno = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Responsable", x => x.ResponsableId);
                });

            migrationBuilder.CreateTable(
                name: "Rol",
                schema: "Reserva",
                columns: table => new
                {
                    RolId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Descripción = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rol", x => x.RolId);
                });

            migrationBuilder.CreateTable(
                name: "Tipo_Equipo",
                schema: "Reserva",
                columns: table => new
                {
                    Tipo_EquipoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tipo_Equipo", x => x.Tipo_EquipoId);
                });

            migrationBuilder.CreateTable(
                name: "Area",
                schema: "Reserva",
                columns: table => new
                {
                    AreaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Descripción = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Area", x => x.AreaId);
                    table.ForeignKey(
                        name: "FK_Area_Empresa",
                        column: x => x.EmpresaId,
                        principalSchema: "Reserva",
                        principalTable: "Empresa",
                        principalColumn: "EmpresaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Equipo",
                schema: "Reserva",
                columns: table => new
                {
                    EquipoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    Disponible = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ResponsableId = table.Column<int>(type: "int", nullable: false),
                    Tipo_EquipoId = table.Column<int>(type: "int", nullable: false),
                    Fecha_Inicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Fecha_Fin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ImagenUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagenJson = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipo", x => x.EquipoId);
                    table.ForeignKey(
                        name: "FK_Equipo_Responsable",
                        column: x => x.ResponsableId,
                        principalSchema: "Reserva",
                        principalTable: "Responsable",
                        principalColumn: "ResponsableId");
                    table.ForeignKey(
                        name: "FK_Equipo_Tipo_Equipo",
                        column: x => x.Tipo_EquipoId,
                        principalSchema: "Reserva",
                        principalTable: "Tipo_Equipo",
                        principalColumn: "Tipo_EquipoId");
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                schema: "Reserva",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    SegundoNombre = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    ApellidoPaterno = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    ApellidoMaterno = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Correo = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Username = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    RolId = table.Column<int>(type: "int", nullable: false),
                    AreaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.UsuarioId);
                    table.ForeignKey(
                        name: "FK_Usuario_Area",
                        column: x => x.AreaId,
                        principalSchema: "Reserva",
                        principalTable: "Area",
                        principalColumn: "AreaId");
                    table.ForeignKey(
                        name: "FK_Usuario_Rol",
                        column: x => x.RolId,
                        principalSchema: "Reserva",
                        principalTable: "Rol",
                        principalColumn: "RolId");
                });

            migrationBuilder.CreateTable(
                name: "Area_Disponible",
                schema: "Reserva",
                columns: table => new
                {
                    Area_DisponibleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    EquipoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Area_Disponible", x => x.Area_DisponibleId);
                    table.ForeignKey(
                        name: "FK_Area_Disponible_Area",
                        column: x => x.AreaId,
                        principalSchema: "Reserva",
                        principalTable: "Area",
                        principalColumn: "AreaId");
                    table.ForeignKey(
                        name: "FK_Area_Disponible_Equipo",
                        column: x => x.EquipoId,
                        principalSchema: "Reserva",
                        principalTable: "Equipo",
                        principalColumn: "EquipoId");
                });

            migrationBuilder.CreateTable(
                name: "Reserva",
                schema: "Reserva",
                columns: table => new
                {
                    ReservaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipoId = table.Column<int>(type: "int", nullable: true),
                    Fecha_Inicio = table.Column<DateTime>(type: "datetime", nullable: false),
                    Fecha_Fin = table.Column<DateTime>(type: "datetime", nullable: true),
                    Indefinido = table.Column<bool>(type: "bit", nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    Estado = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "Pendiente"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    Ubicación = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    Latitud = table.Column<double>(type: "float", nullable: true),
                    Longitud = table.Column<double>(type: "float", nullable: true),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    ConductorId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reserva", x => x.ReservaId);
                    table.ForeignKey(
                        name: "FK_Reserva_Conductor_ConductorId",
                        column: x => x.ConductorId,
                        principalSchema: "Reserva",
                        principalTable: "Conductor",
                        principalColumn: "ConductorId");
                    table.ForeignKey(
                        name: "FK_Reserva_Equipo",
                        column: x => x.EquipoId,
                        principalSchema: "Reserva",
                        principalTable: "Equipo",
                        principalColumn: "EquipoId");
                    table.ForeignKey(
                        name: "FK_Reserva_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalSchema: "Reserva",
                        principalTable: "Usuario",
                        principalColumn: "UsuarioId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Area_EmpresaId",
                schema: "Reserva",
                table: "Area",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Area_Disponible_AreaId",
                schema: "Reserva",
                table: "Area_Disponible",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Area_Disponible_EquipoId",
                schema: "Reserva",
                table: "Area_Disponible",
                column: "EquipoId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipo_ResponsableId",
                schema: "Reserva",
                table: "Equipo",
                column: "ResponsableId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipo_Tipo_EquipoId",
                schema: "Reserva",
                table: "Equipo",
                column: "Tipo_EquipoId");

            migrationBuilder.CreateIndex(
                name: "IX_Reserva_ConductorId",
                schema: "Reserva",
                table: "Reserva",
                column: "ConductorId");

            migrationBuilder.CreateIndex(
                name: "IX_Reserva_EquipoId",
                schema: "Reserva",
                table: "Reserva",
                column: "EquipoId");

            migrationBuilder.CreateIndex(
                name: "IX_Reserva_UsuarioId",
                schema: "Reserva",
                table: "Reserva",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_AreaId",
                schema: "Reserva",
                table: "Usuario",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_RolId",
                schema: "Reserva",
                table: "Usuario",
                column: "RolId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Area_Disponible",
                schema: "Reserva");

            migrationBuilder.DropTable(
                name: "Reserva",
                schema: "Reserva");

            migrationBuilder.DropTable(
                name: "Conductor",
                schema: "Reserva");

            migrationBuilder.DropTable(
                name: "Equipo",
                schema: "Reserva");

            migrationBuilder.DropTable(
                name: "Usuario",
                schema: "Reserva");

            migrationBuilder.DropTable(
                name: "Responsable",
                schema: "Reserva");

            migrationBuilder.DropTable(
                name: "Tipo_Equipo",
                schema: "Reserva");

            migrationBuilder.DropTable(
                name: "Area",
                schema: "Reserva");

            migrationBuilder.DropTable(
                name: "Rol",
                schema: "Reserva");

            migrationBuilder.DropTable(
                name: "Empresa",
                schema: "Reserva");
        }
    }
}
