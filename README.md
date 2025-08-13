# Zoco Backend

Backend desarrollado en **ASP.NET Core 7** con Entity Framework Core y JWT Authentication.

## Requisitos

- [.NET 7 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
- [SQL Server LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)
- (Opcional) Visual Studio 2022 o VS Code

## Configuración inicial

1. Clonar el repositorio:

```bash
git clone https://github.com/TU_USUARIO/zoco-backend.git
cd zoco-backend
```

2. Configurar `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ZocoDb;Trusted_Connection=True;"
  },
  "Jwt": {
    "Key": "CLAVE_SECRETA_DEMO",
    "Issuer": "ZocoApp",
    "Audience": "ZocoAppUsers"
  },
  "FrontendOrigin": "http://localhost:5173"
}
```

3. Crear la base de datos:

```bash
dotnet ef database update
```

4. Ejecutar el backend:

```bash
dotnet run
```

La API estará disponible en: **https://localhost:5001** (o el puerto que indique la consola).

## Endpoints principales

- `POST /api/auth/login` → Login y obtención de token JWT
- `POST /api/auth/register` → Registro de usuario
- `GET /api/addresses` → Listado de direcciones del usuario autenticado
- `POST /api/addresses` → Crear nueva dirección
- `PUT /api/addresses/{id}` → Editar dirección existente
- `DELETE /api/addresses/{id}` → Eliminar dirección
- `GET /api/users` → Listado de usuarios (solo Admin)

## CORS

Está configurado para permitir el origen definido en `FrontendOrigin`.

## Tecnologías usadas

- ASP.NET Core 7
- Entity Framework Core
- SQL Server LocalDB
- JWT Authentication
- Swagger para documentación de la API

## Notas

- Cambiar `Jwt:Key` y `ConnectionStrings` en producción.
- Usar `dotnet user-secrets` o variables de entorno para credenciales sensibles.

---
© 2025 ZocoApp Backend
