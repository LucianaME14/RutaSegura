-- RutaSegura: actualizar SOLO filas de "Usuarios" (sin DROP, sin tocar otras tablas).
-- Úsalo en DBeaver cuando ya tienes la BD creada con migraciones o un seed anterior
-- y solo quieres alinear nombres, correos y PasswordHash de demo.
--
-- Clave distinta por cuenta (8 caracteres; ver comentario en rutasegura_seed_completo.sql).
-- Cierra el backend si el .db está bloqueado.
--
-- REQUISITO: que existan filas con "Id" 1..10 (o edita los WHERE "Id" a los que tengas).
-- Si tienes menos usuarios, comenta las líneas que no apliquen o usa el bloque B más abajo.
--
-- A) Por Id (recomendado si tus Ids coinciden con el seed original)

UPDATE "Usuarios" SET
  "Nombre" = 'Maria L.',
  "Email" = 'maria@admin.com',
  "PasswordHash" = '100000.Z+iWQobBLJrFUofjwHQ+fQ==.fd/KB3uNYnNGJuhTrvsfOZjugVfwoySNvI9OEoZZqss=',
  "Telefono" = '+51 100 000 001',
  "Rol" = 'Administrador',
  "Estado" = 'Activo'
WHERE "Id" = 1;

UPDATE "Usuarios" SET
  "Nombre" = 'Juan P.',
  "Email" = 'juan@admin.com',
  "PasswordHash" = '100000.K9XPR8+rG7jBpRpWr4w+sA==.MwzVjC5RdE4xxABMix6f3qau9DpgBUcKukrvNHFSgPA=',
  "Telefono" = '+51 100 000 002',
  "Rol" = 'Administrador',
  "Estado" = 'Activo'
WHERE "Id" = 2;

UPDATE "Usuarios" SET
  "Nombre" = 'Carlos M.',
  "Email" = 'carlos@admin.com',
  "PasswordHash" = '100000.GUA4BzjuNvXPCL5eM/Nx8A==.ViH0RYpc4BHw7ydlWlNR8u7rx2GDiVxRLrQo7MH916M=',
  "Telefono" = '+51 100 000 003',
  "Rol" = 'Administrador',
  "Estado" = 'Activo'
WHERE "Id" = 3;

UPDATE "Usuarios" SET
  "Nombre" = 'Luciana R.',
  "Email" = 'luciana@admin.com',
  "PasswordHash" = '100000.pcMmiQJIkorVFU1jh1KP9A==.T6h0ymgplgepJ2yLzm8dG66XyrjLTZl/XeQQjcog0I8=',
  "Telefono" = '+51 100 000 004',
  "Rol" = 'Administrador',
  "Estado" = 'Activo'
WHERE "Id" = 4;

UPDATE "Usuarios" SET
  "Nombre" = 'Pedro G.',
  "Email" = 'pedro@admin.com',
  "PasswordHash" = '100000.+2nmsBtZ4jmgVUvXYOtjBQ==.GRNH2A63kbvXOPPRPEyv3KYq3A41VLtzZyx+7R5A4Ic=',
  "Telefono" = '+51 100 000 005',
  "Rol" = 'Administrador',
  "Estado" = 'Activo'
WHERE "Id" = 5;

UPDATE "Usuarios" SET
  "Nombre" = 'Lucia M.',
  "Email" = 'lucia@usuario.com',
  "PasswordHash" = '100000.yxE5TeTNWucZkd/pMDKhQA==.+ENaxFU5/RqeA+tMqzSHqccHwagRcpnFdItDaH1QZL4=',
  "Telefono" = '+51 200 000 001',
  "Rol" = 'Usuario',
  "Estado" = 'Activo'
WHERE "Id" = 6;

UPDATE "Usuarios" SET
  "Nombre" = 'Pablo R.',
  "Email" = 'pablo@usuario.com',
  "PasswordHash" = '100000.1FlbIoZHmZ09To0amQWrVQ==.TBl2HMp++E48rV/EXAQkltaZ9qSOESqjNr73nWDbkhk=',
  "Telefono" = '+51 200 000 002',
  "Rol" = 'Usuario',
  "Estado" = 'Activo'
WHERE "Id" = 7;

UPDATE "Usuarios" SET
  "Nombre" = 'Carmen V.',
  "Email" = 'carmen@usuario.com',
  "PasswordHash" = '100000.MMw/MbZFcgBF/be0byufUw==.vKaxowQp4bIDnsFDaJzm7HCpi1EADxo8PP058fzSl8w=',
  "Telefono" = '+51 200 000 003',
  "Rol" = 'Usuario',
  "Estado" = 'Activo'
WHERE "Id" = 8;

UPDATE "Usuarios" SET
  "Nombre" = 'Diego F.',
  "Email" = 'diego@usuario.com',
  "PasswordHash" = '100000.9q001wpSJdFDRwB8oYeT9Q==.lY1ESIAGJ40PRPn4WaDaGGVEfyAxpHoT3kSvrXE8dGY=',
  "Telefono" = '+51 200 000 004',
  "Rol" = 'Usuario',
  "Estado" = 'Activo'
WHERE "Id" = 9;

UPDATE "Usuarios" SET
  "Nombre" = 'Ana S.',
  "Email" = 'ana@usuario.com',
  "PasswordHash" = '100000.1f9lQaSFvPjmOVQk47TnQw==.MzIvl5oKfBlpSWYydXL6tR/ZQUk/3vHXE9MQI4xhL4Q=',
  "Telefono" = '+51 200 000 005',
  "Rol" = 'Usuario',
  "Estado" = 'Activo'
WHERE "Id" = 10;

-- B) Si aún tienes los correos viejos (superadmin, soporte, etc.) y los Ids no son 1-5,
--    descomenta y ejecuta solo lo que corresponda (un UPDATE por fila afectada).

-- UPDATE "Usuarios" SET "Nombre"='Maria L.', "Email"='maria@admin.com', "PasswordHash"='100000.Z+iWQobBLJrFUofjwHQ+fQ==.fd/KB3uNYnNGJuhTrvsfOZjugVfwoySNvI9OEoZZqss=', "Telefono"='+51 100 000 001' WHERE LOWER("Email") = 'superadmin@admin.com';
-- (mismos PasswordHash que arriba para juan..pedro segun corresponda)
