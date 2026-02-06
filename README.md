# Portfolio (ASP.NET Core + Angular)

## Requirements
- .NET SDK 9
- Node.js **LTS** (recommended: 22.x). Angular 21 does not support Node 25 (build may crash).

## Docker (recommended)
If you have Docker Desktop installed, you can run the whole app without installing .NET/Node locally.

1) Create `.env` (not committed to git):
```bash
cp .env.example .env
```
Update at least `MSSQL_SA_PASSWORD` in `.env`.

2) Run:
```bash
docker compose up --build
```

Then open:
- `http://localhost:8080/` (MVC views)
- `http://localhost:8080/app/` (Angular SPA demo)

The app uses **EF Core + SQL Server** in Docker Compose and seeds the database on first run.

SQL Server is bound to `127.0.0.1:1433` (localhost only) by default.

Alternatively (without Compose):
```bash
docker build -t portfolio .
docker run --rm -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Production -e EmailSettings__Enabled=false portfolio
```

## Database (EF Core + SQL Server)
If `ConnectionStrings:Portfolio` is set, the app reads portfolio content from SQL Server via EF Core and seeds it automatically.

- Docker Compose sets the connection string automatically (service name `mssql`).
- For local run you can point it at your own SQL Server instance:
  - `ConnectionStrings__Portfolio="Server=localhost,1433;Database=portfolio;User Id=sa;Password=...;TrustServerCertificate=True"`

To open the database in a GUI client:
- Host: `localhost`
- Port: `1433`
- User: `sa`
- Password: value of `MSSQL_SA_PASSWORD` from your `.env`
- Database: `portfolio` (optional; can connect to `master` too)

## Admin panel (internal)
There is a small internal admin UI for editing portfolio data stored in SQL Server.

- By default it is disabled (`Admin:Enabled=false`) and returns 404.
- Enable it via environment variables:
  - `Admin__Enabled=true`
  - `Admin__Password=NikonD5300@`
- Open `http://localhost:8080/admin/login` and log in.

In Docker Compose you can enable it via `.env`:
- `ADMIN_ENABLED=true`
- `ADMIN_PASSWORD=...`

Admin pages:
- `GET /admin` (dashboard)
- `GET /admin/profile`
- `GET /admin/technologies`
- `GET /admin/social-links`

## Run (backend)
```bash
dotnet run
```

## Angular SPA demo
- Open `http://localhost:<port>/app/`
- The SPA fetches data from `GET /api/portfolio/*`

## Angular dev server (optional)
Run backend on `http://localhost:5017` (launchSettings `http` profile), then:
```bash
cd ClientApp
npm start
```

## Observability
- Health checks:
  - `GET /health`
  - `GET /health/live`
  - `GET /health/ready`
- Basic telemetry (counters + duration) for CV sending is emitted via `System.Diagnostics.Metrics` under `Portfolio` meter.

## Security
- Security headers + Content Security Policy are added globally.
- Rate limiting is enabled for `POST /Home/Contact` (CV e-mail form).

## Tests
```bash
dotnet test
```

## Build Angular (optional)
By default `dotnet build` does **not** build the Angular app (to keep backend builds fast and independent from Node).

To build Angular as part of the .NET build:
```bash
dotnet build -p:BuildClientAppOnBuild=true
```

Angular output is served under `/app` (from `wwwroot/app/browser`).

If `/app` shows "Angular app not built", build the client:
```bash
cd ClientApp
npm ci
npm run build
```

## CV e-mail sending
CV sending is controlled by `EmailSettings.Enabled` (default: `false`).

For local secrets use user-secrets or environment variables; when enabled, Host/User/Password must be provided.
