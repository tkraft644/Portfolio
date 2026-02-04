# Portfolio (ASP.NET Core + Angular)

## Requirements
- .NET SDK 9
- Node.js **LTS** (recommended: 22.x). Angular 21 does not support Node 25 (build may crash).

## Docker (recommended)
If you have Docker Desktop installed, you can run the whole app without installing .NET/Node locally.

```bash
docker compose up --build
```

Then open:
- `http://localhost:8080/` (MVC views)
- `http://localhost:8080/app/` (Angular SPA demo)

Alternatively (without Compose):
```bash
docker build -t portfolio .
docker run --rm -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Production -e EmailSettings__Enabled=false portfolio
```

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
