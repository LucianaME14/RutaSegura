#!/bin/bash
# Asegurar que el directorio de la base de datos existe y tiene permisos
mkdir -p /var/data
chmod 755 /var/data

# Ejecutar la aplicación
exec dotnet RutaSegura.dll