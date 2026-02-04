# syntax=docker/dockerfile:1

FROM node:22-bookworm-slim AS client-build
WORKDIR /src/ClientApp

COPY ClientApp/package.json ClientApp/package-lock.json ./
RUN npm ci

WORKDIR /src
COPY ClientApp ./ClientApp
COPY wwwroot ./wwwroot
WORKDIR /src/ClientApp
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Portfolio.csproj ./
RUN dotnet restore "Portfolio.csproj"

COPY . ./
COPY --from=client-build /src/wwwroot/app ./wwwroot/app

RUN dotnet publish "Portfolio.csproj" -c Release -o /app/publish -p:ClientAppBuilt=true

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "Portfolio.dll"]
