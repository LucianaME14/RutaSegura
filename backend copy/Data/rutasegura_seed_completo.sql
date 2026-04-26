-- RutaSegura: script completo (DROP + esquema + datos demo) / SQLite + EF
-- Cada cuenta con clave distinta (8 chars; API acepta 8-11):
--   maria@admin.com PassM01! | juan@admin.com PassJ02! | carlos@admin.com PassC03! | luciana@admin.com PassL04! | pedro@admin.com PassP05!
--   lucia@usuario.com UserU01! | pablo@usuario.com UserU02! | carmen@usuario.com UserU03! | diego@usuario.com UserU04! | ana@usuario.com UserU05!
-- DBeaver: copia de seguridad del .db, ejecuta script entero, cierra la API.
PRAGMA foreign_keys = OFF;
-- Orden: hijos -> padres. Incluye historial de migraciones si existía.
DROP TABLE IF EXISTS "RutasHistorial";
DROP TABLE IF EXISTS "UbicacionesGuardadas";
DROP TABLE IF EXISTS "Reportes";
DROP TABLE IF EXISTS "Contactos";
DROP TABLE IF EXISTS "Sesiones";
DROP TABLE IF EXISTS "Proyectos";
DROP TABLE IF EXISTS "Catalogos";
DROP TABLE IF EXISTS "Usuarios";
DROP TABLE IF EXISTS "__EFMigrationsHistory";
-- Tablas (equivalente a 20260424133412_InitialSQLite + 20260424180914 + 20260424195608)
CREATE TABLE "Catalogos" (
  "Id" INTEGER NOT NULL CONSTRAINT "PK_Catalogos" PRIMARY KEY AUTOINCREMENT,
  "Tipo" TEXT NOT NULL,
  "Codigo" TEXT NOT NULL,
  "Nombre" TEXT NOT NULL,
  "Descripcion" TEXT NULL,
  "Activo" INTEGER NOT NULL,
  "CreadoEn" TEXT NOT NULL,
  "ActualizadoEn" TEXT NULL
);
CREATE UNIQUE INDEX "IX_Catalogos_Tipo_Codigo" ON "Catalogos" ("Tipo", "Codigo");
CREATE TABLE "Proyectos" (
  "Id" INTEGER NOT NULL CONSTRAINT "PK_Proyectos" PRIMARY KEY AUTOINCREMENT,
  "Nombre" TEXT NOT NULL,
  "Descripcion" TEXT NULL,
  "Estado" TEXT NOT NULL,
  "FechaInicio" TEXT NULL,
  "FechaFin" TEXT NULL,
  "CreadoEn" TEXT NOT NULL
);
CREATE TABLE "Usuarios" (
  "Id" INTEGER NOT NULL CONSTRAINT "PK_Usuarios" PRIMARY KEY AUTOINCREMENT,
  "Nombre" TEXT NOT NULL,
  "Email" TEXT NOT NULL,
  "PasswordHash" TEXT NOT NULL,
  "Telefono" TEXT NULL,
  "Rol" TEXT NOT NULL,
  "Estado" TEXT NOT NULL,
  "FechaRegistro" TEXT NOT NULL
);
CREATE UNIQUE INDEX "IX_Usuarios_Email" ON "Usuarios" ("Email");
CREATE TABLE "Contactos" (
  "Id" INTEGER NOT NULL CONSTRAINT "PK_Contactos" PRIMARY KEY AUTOINCREMENT,
  "UsuarioId" INTEGER NOT NULL,
  "Nombre" TEXT NOT NULL,
  "Telefono" TEXT NOT NULL,
  "Email" TEXT NULL,
  "Parentesco" TEXT NULL,
  "Prioridad" INTEGER NOT NULL,
  "EsPrincipal" INTEGER NOT NULL,
  "CreadoEn" TEXT NOT NULL,
  CONSTRAINT "FK_Contactos_Usuarios_UsuarioId" FOREIGN KEY ("UsuarioId") REFERENCES "Usuarios" ("Id") ON DELETE CASCADE
);
CREATE INDEX "IX_Contactos_UsuarioId" ON "Contactos" ("UsuarioId");
CREATE TABLE "Reportes" (
  "Id" INTEGER NOT NULL CONSTRAINT "PK_Reportes" PRIMARY KEY AUTOINCREMENT,
  "TipoIncidente" TEXT NOT NULL,
  "Ubicacion" TEXT NOT NULL,
  "Latitud" TEXT NULL,
  "Longitud" TEXT NULL,
  "Descripcion" TEXT NULL,
  "UrlFotoEvidencia" TEXT NULL,
  "FechaReporte" TEXT NOT NULL,
  "Estado" TEXT NOT NULL,
  "EsAnonimo" INTEGER NOT NULL,
  "UsuarioId" INTEGER NOT NULL,
  "CatalogoId" INTEGER NULL,
  "ProyectoId" INTEGER NULL,
  "NivelConfianzaIA" REAL NOT NULL,
  CONSTRAINT "FK_Reportes_Usuarios_UsuarioId" FOREIGN KEY ("UsuarioId") REFERENCES "Usuarios" ("Id") ON DELETE CASCADE,
  CONSTRAINT "FK_Reportes_Catalogos_CatalogoId" FOREIGN KEY ("CatalogoId") REFERENCES "Catalogos" ("Id") ON DELETE SET NULL,
  CONSTRAINT "FK_Reportes_Proyectos_ProyectoId" FOREIGN KEY ("ProyectoId") REFERENCES "Proyectos" ("Id") ON DELETE SET NULL
);
CREATE INDEX "IX_Reportes_UsuarioId" ON "Reportes" ("UsuarioId");
CREATE INDEX "IX_Reportes_CatalogoId" ON "Reportes" ("CatalogoId");
CREATE INDEX "IX_Reportes_ProyectoId" ON "Reportes" ("ProyectoId");
CREATE TABLE "Sesiones" (
  "Id" INTEGER NOT NULL CONSTRAINT "PK_Sesiones" PRIMARY KEY AUTOINCREMENT,
  "UsuarioId" INTEGER NOT NULL,
  "TokenJti" TEXT NOT NULL,
  "RefreshTokenHash" TEXT NULL,
  "IpAddress" TEXT NULL,
  "UserAgent" TEXT NULL,
  "Origen" TEXT NOT NULL,
  "CreadaEn" TEXT NOT NULL,
  "ExpiraEn" TEXT NOT NULL,
  "CerradaEn" TEXT NULL,
  "Estado" TEXT NOT NULL,
  CONSTRAINT "FK_Sesiones_Usuarios_UsuarioId" FOREIGN KEY ("UsuarioId") REFERENCES "Usuarios" ("Id") ON DELETE CASCADE
);
CREATE INDEX "IX_Sesiones_UsuarioId" ON "Sesiones" ("UsuarioId");
CREATE UNIQUE INDEX "IX_Sesiones_TokenJti" ON "Sesiones" ("TokenJti");
CREATE TABLE "UbicacionesGuardadas" (
  "Id" INTEGER NOT NULL CONSTRAINT "PK_UbicacionesGuardadas" PRIMARY KEY AUTOINCREMENT,
  "UsuarioId" INTEGER NOT NULL,
  "Etiqueta" TEXT NOT NULL,
  "Direccion" TEXT NOT NULL,
  "Latitud" TEXT NULL,
  "Longitud" TEXT NULL,
  "Icono" TEXT NULL,
  "Orden" INTEGER NOT NULL,
  "CreadoEn" TEXT NOT NULL,
  CONSTRAINT "FK_UbicacionesGuardadas_Usuarios_UsuarioId" FOREIGN KEY ("UsuarioId") REFERENCES "Usuarios" ("Id") ON DELETE CASCADE
);
CREATE INDEX "IX_UbicacionesGuardadas_UsuarioId" ON "UbicacionesGuardadas" ("UsuarioId");
CREATE TABLE "RutasHistorial" (
  "Id" INTEGER NOT NULL CONSTRAINT "PK_RutasHistorial" PRIMARY KEY AUTOINCREMENT,
  "UsuarioId" INTEGER NOT NULL,
  "OrigenTexto" TEXT NOT NULL,
  "DestinoTexto" TEXT NOT NULL,
  "Modo" TEXT NOT NULL,
  "MinutosAprox" INTEGER NOT NULL,
  "KmAprox" REAL NOT NULL,
  "RutaReferencia" TEXT NULL,
  "CreadoEn" TEXT NOT NULL,
  CONSTRAINT "FK_RutasHistorial_Usuarios_UsuarioId" FOREIGN KEY ("UsuarioId") REFERENCES "Usuarios" ("Id") ON DELETE CASCADE
);
CREATE INDEX "IX_RutasHistorial_UsuarioId_CreadoEn" ON "RutasHistorial" ("UsuarioId", "CreadoEn");
CREATE TABLE "__EFMigrationsHistory" (
  "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
  "ProductVersion" TEXT NOT NULL
);
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES
  ('20260424133412_InitialSQLite', '8.0.0'),
  ('20260424180914_AddReporteAnonimoUbicaciones', '8.0.0'),
  ('20260424195608_AddRutasHistorial', '8.0.0');
-- Datos: catálogo de tipos (Tipo + Codigo único)
INSERT INTO "Catalogos" ("Id", "Tipo", "Codigo", "Nombre", "Descripcion", "Activo", "CreadoEn", "ActualizadoEn")
VALUES
  (1, 'INCIDENTE', 'ACC_001', 'Accidente de tránsito', 'Accidente en intersección', 1, '2024-10-20T00:00:00', '2024-10-20T00:00:00'),
  (2, 'INCIDENTE', 'CERR_01', 'Calle cerrada / Obra', 'Bloqueo o desvío', 1, '2024-10-20T00:00:00', '2024-10-20T00:00:00'),
  (3, 'INCIDENTE', 'ALERT_1', 'Evacuación / Sirena', 'Movilización o alerta pública', 1, '2024-10-20T00:00:00', '2024-10-20T00:00:00'),
  (4, 'INCIDENTE', 'CORTE', 'Corte de luz', 'Corte o falla eléctrica', 1, '2024-10-20T00:00:00', '2024-10-20T00:00:00'),
  (5, 'INCIDENTE', 'SEM', 'Semáforo sin servicio', 'Semáforo apagado o inoperativo', 1, '2024-10-20T00:00:00', '2024-10-20T00:00:00'),
  (6, 'INCIDENTE', 'INC_01', 'Riesgo de incendio', 'Humo o riesgo en edificación', 1, '2024-10-20T00:00:00', '2024-10-20T00:00:00');
INSERT INTO "Proyectos" ("Id", "Nombre", "Descripcion", "Estado", "FechaInicio", "FechaFin", "CreadoEn")
VALUES
  (1, 'Ruta Protegida: Zona 7', 'Monitoreo y coordinación con municipalidad 2024–2025', 'En curso', '2024-10-10T00:00:00', '2025-12-20T00:00:00', '2024-10-20T00:00:00');
-- CUENTAS: clave unica por usuario (PasswordService PBKDF2)
INSERT INTO "Usuarios" ("Id", "Nombre", "Email", "PasswordHash", "Telefono", "Rol", "Estado", "FechaRegistro")
VALUES
  (1, 'Maria L.', 'maria@admin.com',
   '100000.Z+iWQobBLJrFUofjwHQ+fQ==.fd/KB3uNYnNGJuhTrvsfOZjugVfwoySNvI9OEoZZqss=',
   '+51 100 000 001', 'Administrador', 'Activo', '2024-10-20T00:00:00'),
  (2, 'Juan P.', 'juan@admin.com',
   '100000.K9XPR8+rG7jBpRpWr4w+sA==.MwzVjC5RdE4xxABMix6f3qau9DpgBUcKukrvNHFSgPA=',
   '+51 100 000 002', 'Administrador', 'Activo', '2024-10-20T00:00:00'),
  (3, 'Carlos M.', 'carlos@admin.com',
   '100000.GUA4BzjuNvXPCL5eM/Nx8A==.ViH0RYpc4BHw7ydlWlNR8u7rx2GDiVxRLrQo7MH916M=',
   '+51 100 000 003', 'Administrador', 'Activo', '2024-10-20T00:00:00'),
  (4, 'Luciana R.', 'luciana@admin.com',
   '100000.pcMmiQJIkorVFU1jh1KP9A==.T6h0ymgplgepJ2yLzm8dG66XyrjLTZl/XeQQjcog0I8=',
   '+51 100 000 004', 'Administrador', 'Activo', '2024-10-20T00:00:00'),
  (5, 'Pedro G.', 'pedro@admin.com',
   '100000.+2nmsBtZ4jmgVUvXYOtjBQ==.GRNH2A63kbvXOPPRPEyv3KYq3A41VLtzZyx+7R5A4Ic=',
   '+51 100 000 005', 'Administrador', 'Activo', '2024-10-20T00:00:00'),
  (6, 'Lucia M.', 'lucia@usuario.com',
   '100000.yxE5TeTNWucZkd/pMDKhQA==.+ENaxFU5/RqeA+tMqzSHqccHwagRcpnFdItDaH1QZL4=',
   '+51 200 000 001', 'Usuario', 'Activo', '2024-10-20T00:00:00'),
  (7, 'Pablo R.', 'pablo@usuario.com',
   '100000.1FlbIoZHmZ09To0amQWrVQ==.TBl2HMp++E48rV/EXAQkltaZ9qSOESqjNr73nWDbkhk=',
   '+51 200 000 002', 'Usuario', 'Activo', '2024-10-20T00:00:00'),
  (8, 'Carmen V.', 'carmen@usuario.com',
   '100000.MMw/MbZFcgBF/be0byufUw==.vKaxowQp4bIDnsFDaJzm7HCpi1EADxo8PP058fzSl8w=',
   '+51 200 000 003', 'Usuario', 'Activo', '2024-10-20T00:00:00'),
  (9, 'Diego F.', 'diego@usuario.com',
   '100000.9q001wpSJdFDRwB8oYeT9Q==.lY1ESIAGJ40PRPn4WaDaGGVEfyAxpHoT3kSvrXE8dGY=',
   '+51 200 000 004', 'Usuario', 'Activo', '2024-10-20T00:00:00'),
  (10, 'Ana S.', 'ana@usuario.com',
   '100000.1f9lQaSFvPjmOVQk47TnQw==.MzIvl5oKfBlpSWYydXL6tR/ZQUk/3vHXE9MQI4xhL4Q=',
   '+51 200 000 005', 'Usuario', 'Activo', '2024-10-20T00:00:00');
INSERT INTO "Contactos" ("Id", "UsuarioId", "Nombre", "Telefono", "Email", "Parentesco", "Prioridad", "EsPrincipal", "CreadoEn")
VALUES
  (1, 6, 'Maria Lopez', '+51 200 100 100', 'contacto1@usuario.com', 'Hermana', 1, 1, '2024-10-20T00:00:00'),
  (2, 7, 'Carlos Ruiz', '+51 200 100 200', 'contacto2@usuario.com', 'Amigo', 2, 0, '2024-10-20T00:00:00');
-- EsAnonimo: 0 = no, 1 = reporte anónimo (aunque siga ligado a UsuarioId en el esquema actual)
INSERT INTO "Reportes" ("Id", "TipoIncidente", "Ubicacion", "Latitud", "Longitud", "Descripcion", "UrlFotoEvidencia", "FechaReporte", "Estado", "EsAnonimo", "UsuarioId", "CatalogoId", "ProyectoId", "NivelConfianzaIA")
VALUES
  (1, 'Cierre vial en Av. Principal', 'Cerca de Miraflores, Lima', '-12.1190', '-77.0290',
   'Descripcion detallada del cierre. Obra pública. ', NULL, '2024-10-20T00:00:00', 'En revisión', 0, 6, 1, 1, 0.8),
  (2, 'Alerta: Semáforo inoperante', 'Av. Larco, cuadra 3', NULL, NULL,
   'Cruce peligroso, semáforo en intermitente.', 'https://ejemplo.com/fotos/sem1.jpg', '2024-10-20T00:00:00', 'En revisión', 0, 1, 5, 1, 0.4),
  (3, 'Obstruccion en pista', 'Calle 10 cerca a parque', NULL, NULL,
   'Carga o vehiculo estorbando la via publica (ejemplo de borrador).', NULL, '2024-10-20T00:00:00', 'Borrador', 0, 1, 2, NULL, 0.0),
  (4, 'Incendio / humo (reporte con anonimato)', 'Edificio 25B', '-12.1220', '-77.0210',
   'Reporte marcado como anonimo a efectos de prueba (EsAnonimo=1).', NULL, '2024-10-20T00:00:00', 'Enviado', 1, 6, 4, 1, 0.0);
-- Reinicio de secuencias de autoincremento (evita colisionar si se inserta después sin Id)
DELETE FROM "sqlite_sequence";
INSERT INTO "sqlite_sequence" ("name", "seq")
VALUES
  ('Catalogos', 6), ('Proyectos', 1), ('Usuarios', 10), ('Contactos', 2), ('Reportes', 4);
PRAGMA foreign_keys = ON;