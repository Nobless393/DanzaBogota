# DanzaBogotá API

API REST en **.NET 8** para gestionar eventos de danza cultural en Bogotá: eventos, grupos,
participantes, inscripciones, categorías y localidades, con autenticación JWT y roles.

Proyecto de grado — Ingeniería de Sistemas, UNAD.

## Tecnologías

- C# · ASP.NET Core 8 Web API
- Entity Framework Core 8 + SQL Server (migraciones y datos semilla) · Dapper
- ASP.NET Core Identity + JWT Bearer, con roles **Admin**, **Gestor** y **Grupo**
- Swagger / OpenAPI

## Arquitectura

Solución en capas siguiendo arquitectura limpia. Las dependencias apuntan hacia el dominio:

| Capa | Proyectos | Responsabilidad |
|---|---|---|
| Domain | `Domain.Entity`, `Domain.Core`, `Domain.Interface` | Entidades y contratos del negocio |
| Application | `Application.Main`, `Application.Interface`, `Application.Dto`, `Application.Validator` | Casos de uso y DTOs |
| Infrastructure | `Infrastructure.Data`, `Infrastructure.Repository`, `Infrastructure.Interface` | EF Core, migraciones, seeders y repositorio genérico |
| Transversal | `Transversal.Common`, `Transversal.Logging`, `Transversal.Mapper` | JWT y utilidades compartidas |
| Presentación | `Services.WebApi` | Controllers, configuración y arranque |

## Endpoints

Nueve controllers bajo `api/[controller]`: `Auth`, `User`, `Event`, `EventType`, `Group`,
`Participant`, `Inscription`, `Category` y `Locality`. La documentación completa está en Swagger.

## Cómo ejecutarlo

**Requisitos:** .NET 8 SDK y SQL Server (Express sirve).

1. Clona el repositorio y entra al proyecto de la API:

   ```bash
   git clone https://github.com/Nobless393/DanzaBogota.git
   cd DanzaBogota/DanzaBogota.Services.WebApi
   ```

2. Configura los secretos locales. **No se guardan en el repositorio**:

   ```bash
   dotnet user-secrets set "Jwt:Key" "<clave-aleatoria-de-al-menos-32-caracteres>"
   dotnet user-secrets set "Seed:AdminEmail" "admin@ejemplo.com"
   dotnet user-secrets set "Seed:AdminPassword" "<contraseña-segura>"
   ```

   Si tu SQL Server no es `localhost\SQLEXPRESS` con autenticación de Windows, cambia también la
   cadena de conexión:

   ```bash
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<tu-cadena-de-conexión>"
   ```

3. Crea la base de datos y arranca la API:

   ```bash
   dotnet ef database update --project ../DanzaBogota.Infrastructure.Data
   dotnet run
   ```

4. Abre Swagger en `https://localhost:7154/swagger`.

Al arrancar se crean los roles, el usuario administrador (si configuraste `Seed:*`) y los datos de
referencia: localidades, categorías y tipos de evento.

En producción usa variables de entorno en lugar de user-secrets (`Jwt__Key`, `Seed__AdminPassword`,
`ConnectionStrings__DefaultConnection`).
