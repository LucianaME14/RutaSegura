FROM node:20-alpine AS frontend-build
WORKDIR /src
COPY package.json package-lock.json* ./
COPY npm-shrinkwrap.json* ./
COPY . .
RUN npm install
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["backend/RutaSegura.csproj", "backend/"]
RUN dotnet restore "backend/RutaSegura.csproj"

COPY . .
RUN dotnet publish "backend/RutaSegura.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
COPY --from=frontend-build /src/dist ./wwwroot

ENV ASPNETCORE_URLS=http://0.0.0.0:$PORT
EXPOSE 80

ENTRYPOINT ["dotnet", "RutaSegura.dll"]
