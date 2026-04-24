using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RutaSegura.Migrations
{
    /// <inheritdoc />
    public partial class AddRutasHistorial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RutasHistorial",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UsuarioId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrigenTexto = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    DestinoTexto = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Modo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    MinutosAprox = table.Column<int>(type: "INTEGER", nullable: false),
                    KmAprox = table.Column<double>(type: "REAL", nullable: false),
                    RutaReferencia = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    CreadoEn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RutasHistorial", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RutasHistorial_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RutasHistorial_UsuarioId_CreadoEn",
                table: "RutasHistorial",
                columns: new[] { "UsuarioId", "CreadoEn" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RutasHistorial");
        }
    }
}
