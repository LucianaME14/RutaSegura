
# Plataforma de Ruta Segura

Este proyecto incluye:

- Frontend en React + Vite
- Backend en .NET 8 (Web API)
- Base de datos SQLite (compatible con DBeaver)
- Soporte para sesiones con JWT y Redis (opcional en local, recomendado en Render)

Proyecto base de diseño: [Figma - Plataforma de Ruta Segura](https://www.figma.com/design/BfhpVYZvQxJOG60Pmxam8l/Plataforma-de-Ruta-Segura)

## Requisitos

- Node.js 18+
- .NET SDK 8+
- (Opcional) DBeaver

## Ejecucion local

### 1) Frontend

```bash
npm i
npm run dev
```

Frontend por defecto: `http://localhost:5173`

### 2) Backend

```bash
dotnet restore backend/RutaSegura.csproj
dotnet build backend/RutaSegura.csproj
dotnet run --project backend/RutaSegura.csproj
```

API por defecto: `https://localhost:xxxx` o `http://localhost:xxxx` (segun tu perfil local).

## Base de datos SQLite (EF Core)

La conexion local esta en `backend/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=rutasegura.db"
}
```

### Crear/aplicar migraciones

Ya existe la migracion inicial (`InitialSQLite`). Si necesitas recrear o actualizar:

```bash
dotnet ef migrations add NombreMigracion --project backend/RutaSegura.csproj --startup-project backend/RutaSegura.csproj
dotnet ef database update --project backend/RutaSegura.csproj --startup-project backend/RutaSegura.csproj
```

## DBeaver (SQLite)

1. Abre DBeaver.
2. Crea una conexion nueva de tipo `SQLite`.
3. Selecciona el archivo `rutasegura.db` en la raiz del proyecto.
4. Conecta y revisa tablas:
   - `Usuarios`
   - `Reportes`
   - `Contactos`
   - `Catalogos`
   - `Sesiones`
   - `Proyectos`

## Endpoints principales

- `POST /api/Auth/register`
- `POST /api/Auth/login`
- `GET /api/Usuarios`
- `GET /api/Contactos/usuario/{usuarioId}`
- `POST /api/Contactos`
- `PUT /api/Contactos/{id}`
- `DELETE /api/Contactos/{id}`
- `GET /api/Session/usuario/{usuarioId}`
- `POST /api/Session/revocar/{id}`
- `GET /api/Reportes`
- `POST /api/Reportes/Crear`
- `POST /api/Reportes/Aprobar/{id}`

## Despliegue en Render

Se agrego `render.yaml` para desplegar:

- `rutasegura-api` (Web Service .NET)
- `rutasegura-redis` (Redis)
- disco persistente en `/var/data` para SQLite

### Pasos

1. Sube el repositorio a GitHub.
2. En Render: `New +` -> `Blueprint`.
3. Selecciona el repositorio.
4. Render detectara `render.yaml` y creara servicios automaticamente.
5. Verifica en variables:
   - `ConnectionStrings__DefaultConnection=Data Source=/var/data/rutasegura.db`
   - `Jwt__Issuer`
   - `Jwt__Audience`
   - `Jwt__Key`
   - `Redis__ConnectionString`

## Redis en local (opcional)

Si no configuras Redis local, la API funciona igual (modo deshabilitado).  
Para habilitar Redis local, define en `backend/appsettings.json` o variables de entorno:

```json
"Redis": {
  "ConnectionString": "localhost:6379"
}
```
  