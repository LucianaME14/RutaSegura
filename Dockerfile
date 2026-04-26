FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["backend/RutaSegura.csproj", "backend/"]
RUN dotnet restore "backend/RutaSegura.csproj"

COPY . .
RUN dotnet publish "backend/RutaSegura.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Copiar script de entrada
COPY docker-entrypoint.sh .
RUN chmod +x docker-entrypoint.sh

# Crear directorio para la base de datos
RUN mkdir -p /var/data && chmod 755 /var/data

ENV ASPNETCORE_URLS=http://0.0.0.0:$PORT
EXPOSE 8080

ENTRYPOINT ["./docker-entrypoint.sh"]
