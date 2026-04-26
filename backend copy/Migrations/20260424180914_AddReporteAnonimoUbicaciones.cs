using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RutaSegura.Migrations
{
    /// <inheritdoc />
    public partial class AddReporteAnonimoUbicaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EsAnonimo",
                table: "Reportes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "UbicacionesGuardadas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UsuarioId = table.Column<int>(type: "INTEGER", nullable: false),
                    Etiqueta = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Direccion = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Latitud = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Longitud = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Icono = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Orden = table.Column<int>(type: "INTEGER", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UbicacionesGuardadas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UbicacionesGuardadas_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UbicacionesGuardadas_UsuarioId",
                table: "UbicacionesGuardadas",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UbicacionesGuardadas");

            migrationBuilder.DropColumn(
                name: "EsAnonimo",
                table: "Reportes");
        }
    }
}
