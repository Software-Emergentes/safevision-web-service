# SafeVision Platform

Sistema de monitoreo de fatiga y gestión de conductores para empresas de transporte.

## Requisitos del Sistema

- .NET 8.0 SDK
- MySQL 8.0 o superior

## Configuración de Base de Datos

Editar el archivo `appsettings.json` con la cadena de conexión de MySQL:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;user=root;password=12345678;database=safevision"
  }
}
```

## Ejecutar la Aplicación

```bash
cd SafeVisionPlatform
dotnet run
```

La aplicación estará disponible en:
- HTTP: http://localhost:5272
- HTTPS: https://localhost:7049
- Swagger UI: https://localhost:7049/swagger

## Ejecutar las Pruebas

Todas las pruebas:
```bash
cd SafeVisionPlatform.Tests
dotnet test
```

Solo pruebas unitarias:
```bash
dotnet test --filter "FullyQualifiedName~UnitTests"
```

Solo pruebas de integración:
```bash
dotnet test --filter "FullyQualifiedName~IntegrationTests"
```

Pruebas por bounded context:
```bash
dotnet test --filter "FullyQualifiedName~Driver"
dotnet test --filter "FullyQualifiedName~IAM"
dotnet test --filter "FullyQualifiedName~Trip"
dotnet test --filter "FullyQualifiedName~Management"
dotnet test --filter "FullyQualifiedName~FatigueMonitoring"
```

## Estructura del Proyecto

```
SafeVisionPlatform/
├── Driver/
├── IAM/
├── Trip/
├── Management/
├── FatigueMonitoring/
└── Shared/

SafeVisionPlatform.Tests/
├── UnitTests/
└── IntegrationTests/
```
