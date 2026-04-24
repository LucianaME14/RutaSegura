using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RutaSegura.Migrations
{
    /// <inheritdoc />
    public partial class ConfigyAlertasAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlertasSistema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Titulo = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Detalle = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Prioridad = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Origen = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    UbicacionRef = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    RiesgoEstimadoPct = table.Column<int>(type: "INTEGER", nullable: false),
                    CreadaEn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertasSistema", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracionSistema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PesoZonasOscurasPct = table.Column<int>(type: "INTEGER", nullable: false),
                    CaducidadReporteMenorHoras = table.Column<int>(type: "INTEGER", nullable: false),
                    AutoAprobarConfianzaMinPct = table.Column<int>(type: "INTEGER", nullable: false),
                    PushNotificacionUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    GoogleMapsKeyAlmacenada = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionSistema", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ConfiguracionSistema",
                columns: new[] { "Id", "AutoAprobarConfianzaMinPct", "CaducidadReporteMenorHoras", "GoogleMapsKeyAlmacenada", "PesoZonasOscurasPct", "PushNotificacionUrl" },
                values: new object[] { 1, 85, 24, null, 40, "https://push.rutasegura.net" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlertasSistema");

            migrationBuilder.DropTable(
                name: "ConfiguracionSistema");
        }
    }
}
