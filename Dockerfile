# Utiliza el SDK de .NET 8 para compilar y el runtime de ASP.NET 8 para ejecutar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia el archivo de proyecto y restaura dependencias
COPY SafeVisionPlatform/SafeVisionPlatform.csproj SafeVisionPlatform/
RUN dotnet restore SafeVisionPlatform/SafeVisionPlatform.csproj

# Copia el resto del código fuente
COPY SafeVisionPlatform/ SafeVisionPlatform/

# Publica la aplicación en modo Release
RUN dotnet publish SafeVisionPlatform/SafeVisionPlatform.csproj -c Release -o /app/publish /p:UseAppHost=false

# Imagen final para producción
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SafeVisionPlatform.dll"]

