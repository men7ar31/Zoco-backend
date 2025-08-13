# Zoco – Backend (.NET 7/9 + EF Core + JWT)

API REST para la prueba técnica FullStack Zoco.
Incluye autenticación JWT, control de ownership por usuario, SessionLogs reales, CORS estricto y un CRUD de Estudios protegido por rol/políticas.
Las ampliaciones implementadas siguen la “Prueba Complementaria” (SessionLogs, ownership, README, CORS/secretos, etc.).

🧱 Tech stack
- ASP.NET Core (minimal hosting, .NET 7/9 compatible)
- Entity Framework Core (SQL Server / Azure SQL)
- JWT (Auth bearer)
- Swagger (dev y opcional en prod por flag)
- CORS (whitelist por dominio)

📦 Arquitectura (alto nivel)
- Controllers/
  - AuthController → login/logout + emisión de JWT y registro de sesiones
  - StudiesController → CRUD de Estudios con ownership (User accede solo a lo propio, Admin a todo)
  - SessionLogsController → [Authorize(Roles="Admin")] listado de sesiones
- Models/ → User, Study, SessionLog
- Data/ → AppDbContext + Migrations/
- Services/ → UsersService (ej.)
- Helpers/ → JwtHelper
- Seed/ → DbSeeder (crea usuarios de prueba)

🔐 Autenticación y roles
- JWT Bearer.
- Claims estándar: nameid (UserId), role (“User”, “Admin”).
- Header: Authorization: Bearer <token>.

Roles:
- Admin: acceso global + /api/sessionlogs
- User: solo a recursos propios (Studies del propio UserId). Probar un 403 intencional al acceder a /api/sessionlogs.

🧭 Endpoints principales
Base URL (prod): https://zoco-sql-srv.database.windows.net

Auth
- POST /api/auth/login
  Body: { "email": "admin@zoco.com", "password": "Admin123!" }
  Respuesta: { token, sessionLogId } y crea SessionLog(FechaInicio)
- POST /api/auth/logout ([Authorize])
  Cierra la última sesión abierta (FechaFin)

Estudios (ownership)
- GET /api/studies ([Authorize])
  User → Solo sus estudios
  Admin → Todos, con filtro opcional ?userId=
- GET /api/studies/{id} ([Authorize])
- POST /api/studies ([Authorize])
- PUT /api/studies/{id} ([Authorize])
- DELETE /api/studies/{id} ([Authorize])

Session Logs (solo Admin)
- GET /api/sessionlogs ([Authorize(Roles="Admin")])

🗃️ Modelo de datos (resumen)
User { Id, Email, PasswordHash, Role, ICollection<Study>, ICollection<SessionLog> }
Study { Id, Title, Institution, CompletedAt?, UserId(FK) }
SessionLog { Id, UserId(FK), FechaInicio, FechaFin?, IpAddress?, UserAgent? }

🌐 CORS
Whitelist por configuración (solo dominios explícitos):

Producción (Azure App Service → Configuration → Application settings):
FrontendOrigins__0 = https://zoco-frontend-kohl.vercel.app

Desarrollo (appsettings.Development.json o User Secrets):
"FrontendOrigins": [ "http://localhost:5173" ]

Orden de middleware (clave para preflight):
UseHttpsRedirection() → UseRouting() → UseCors() → UseAuthentication() → UseAuthorization() → MapControllers()

🧩 Variables de entorno / configuración
No subir secretos a Git. Usa User Secrets (local) y App Settings (Azure).

Obligatorias:
- ConnectionStrings:DefaultConnection → cadena a SQL Server/Azure SQL
- JwtSettings__SecretKey → cadena larga
- JwtSettings__Issuer → ZocoApp
- JwtSettings__Audience → ZocoUsers
- FrontendOrigins__0 → (ver sección CORS)

Opcionales:
- Swagger__Enabled = true (para habilitar Swagger en prod temporalmente)

▶️ Puesta en marcha (local)
1) Dependencias
   dotnet --version
   dotnet tool install --global dotnet-ef

2) Configurar secrets (local)
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=...;Database=ZocoDb;..."
   dotnet user-secrets set "JwtSettings:SecretKey" "una_clave_larga_segura"
   dotnet user-secrets set "JwtSettings:Issuer" "ZocoApp"
   dotnet user-secrets set "JwtSettings:Audience" "ZocoUsers"
   dotnet user-secrets set "FrontendOrigins:0" "http://localhost:5173"

3) Migraciones y BD
   dotnet ef database update

4) Run
   dotnet run
   Swagger (dev): https://localhost:xxxx/swagger

🌱 Datos seed (usuarios de prueba)
El seeder crea dos usuarios:

Rol   | Email            | Password
------|------------------|---------
Admin | admin@zoco.com   | Admin123!
User  | test@zoco.com    | Test123!

Úsalos para probar roles, ownership y 403.

🧪 Pruebas rápidas (curl)
Login:
curl -s -X POST "<BASE>/api/auth/login" -H "Content-Type: application/json" -d '{"email":"admin@zoco.com","password":"Admin123!"}'

Listar estudios (con token):
curl -s "<BASE>/api/studies" -H "Authorization: Bearer <TOKEN>"

Session logs (Admin):
curl -s "<BASE>/api/sessionlogs" -H "Authorization: Bearer <TOKEN>"

✅ Qué se implementó de la complementaria
- SessionLogs reales
- CRUD de Estudios con ownership
- Frontend + roles
- Higiene de secretos
- README con pasos de migraciones, credenciales de prueba y cómo probar

🚀 Deploy (Azure App Service)
1. Crear Azure SQL y App Service (.NET 8 LTS o Windows).
2. En App Service → Configuration:
   - Variables: JwtSettings__*, FrontendOrigins__*, Swagger__Enabled (opcional).
   - Connection strings: DefaultConnection tipo SQLAzure.
3. Reiniciar la app tras cambios.
4. Probar en …azurewebsites.net/swagger (si habilitado) o con Postman.

🧭 Endpoints principales

Base URL (producción):  
https://zocoapp-dvg9crgygwhhbrep.brazilsouth-01.azurewebsites.net/


🐛 Troubleshooting
- CORS / preflight (OPTIONS) 401/404: verificar orden de middleware
- 500.30 al iniciar: revisar logs de App Service y cadena de conexión
- 401 desde frontend: comprobar que el Authorization: Bearer <token> se esté enviando
- 403 esperado: User intentando acceder a endpoint solo para Admin

Licencia
Uso exclusivo para la prueba técnica de Zoco.

