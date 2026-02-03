# Portfolio (ASP.NET Core + Angular)

## Requirements
- .NET SDK 9
- Node.js **LTS** (recommended: 22.x)

## Run (backend)
```bash
dotnet run
```

## Build Angular (optional)
By default `dotnet build` does **not** build the Angular app (to keep backend builds fast and independent from Node).

To build Angular as part of the .NET build:
```bash
dotnet build -p:BuildClientAppOnBuild=true
```

Angular output is served under `/app` (from `wwwroot/app/browser`).

## CV e-mail sending
CV sending is controlled by `EmailSettings.Enabled` (default: `false`).

For local secrets use user-secrets or environment variables; when enabled, Host/User/Password must be provided.
