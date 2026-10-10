# Undercut

[![CI](https://github.com/<your-username>/<your-repo>/actions/workflows/ci.yml/badge.svg)](https://github.com/<your-username>/<your-repo>/actions/workflows/ci.yml)

A multi-tenant motorsport prediction league platform built with ASP.NET Core. Each league is an isolated tenant with its own members, picks, and scoring rules. Players predict race podiums before a server-enforced lock deadline, and results are scored idempotently after each race.

> **Status: early development (week 1 of an 8-week build).** The foundation is in place: solution structure, database schema, tenancy model, and CI. Auth, league endpoints, scoring, and the event-driven notification layer are in progress. See the [roadmap](#roadmap).

_Undercut is an independent portfolio project. It is not affiliated with, endorsed by, or connected to Formula 1 or any racing organization._

## What it demonstrates

- **Tenant isolation by design.** Leagues are the tenant boundary, enforced in two layers: a global EF Core query filter as the safety net, and a membership check as the gate. Non-members get `404` rather than `403`, so league existence is not revealed.
- **Concurrency-safe data model.** Unique indexes protect against duplicate memberships and picks under concurrent requests, and picks carry a row-version token for optimistic concurrency.
- **Server-enforced rules.** The pick lock deadline is checked on the server using an injectable clock, so it is testable.
- **Secure local development.** No secrets in source control; see [Security notes](#security-notes).
- **Tested against real infrastructure.** Integration tests run against a real SQL Server via Testcontainers rather than an in-memory provider.

## Architecture

```
src/
  Undercut.Api              HTTP endpoints, auth, middleware
  Undercut.Domain           Entities and abstractions (no dependencies)
  Undercut.Infrastructure   EF Core, DbContext, migrations
tests/
  Undercut.Tests            xUnit + Testcontainers
```

**Data model:** `User`, `League`, `Membership`, `Pick`, `Driver`, `Race`, `Result`. `Membership` and `Pick` are league-scoped and filtered by tenant. The rest are global.

## Tech stack

- C# / ASP.NET Core Web API
- Entity Framework Core with SQL Server
- JWT bearer authentication
- xUnit and Testcontainers
- Docker Compose for local infrastructure
- GitHub Actions for CI

Planned: Azure App Service or Container Apps, Azure SQL, Key Vault, Application Insights, and Azure Service Bus with a Worker Service.

## Getting started

### Prerequisites

- Current .NET LTS SDK
- Docker Desktop (or another Docker runtime)
- `dotnet-ef` tool: `dotnet tool install --global dotnet-ef`

**Apple Silicon:** the SQL Server image is x86-only. In Docker Desktop, enable _Use Rosetta for x86_64/amd64 emulation_ and allocate at least 4 GB of memory.

### 1. Configure the database password

```bash
cp .env.example .env
# Edit .env and set a strong password, for example:
#   openssl rand -base64 24 | tr -d '=+/'
```

`.env` is gitignored. Never commit it.

### 2. Start SQL Server

```bash
docker compose up -d --wait
```

The database port is bound to `127.0.0.1` only. Data persists in a named volume. Note that SQL Server applies the SA password only when the volume is first created; to reset it, run `docker compose down -v`.

### 3. Set application secrets

Secrets are stored with .NET user-secrets, outside the repository. Use single quotes so your shell does not alter special characters:

```bash
dotnet user-secrets init --project src/Undercut.Api
dotnet user-secrets set 'ConnectionStrings:Default' 'Server=127.0.0.1,1433;Database=Undercut;User Id=sa;Password=<your-password>;TrustServerCertificate=True' --project src/Undercut.Api
dotnet user-secrets set 'Jwt:Key' "$(openssl rand -base64 48)" --project src/Undercut.Api
```

`TrustServerCertificate=True` is for the local container's self-signed certificate only and must not be used in production.

### 4. Apply migrations

```bash
dotnet ef database update -p src/Undercut.Infrastructure -s src/Undercut.Api
```

### 5. Run and test

```bash
dotnet run --project src/Undercut.Api
dotnet test
```

Tests start their own throwaway SQL Server container, so Docker must be running.

## Security notes

- Secrets live in `.env` (gitignored) and user-secrets, never in committed files.
- The local database port is bound to localhost, not all interfaces.
- League roles are stored per membership, not in the JWT, since a user can hold different roles in different leagues.
- Unique database constraints, not application checks alone, are the defense against duplicates under concurrency.
- Cross-tenant writes are blocked in `SaveChangesAsync` as a second layer behind the query filter.

## Roadmap

- [x] Solution structure, CI, local database
- [x] Domain model and EF Core schema with initial migration
- [ ] Auth (register, login, JWT) and tenant middleware
- [ ] League create, join, and list endpoints
- [ ] Tenant-leakage integration tests
- [ ] Picks with server-enforced lock deadline and optimistic concurrency
- [ ] Results import and idempotent scoring with per-league rules
- [ ] Azure deployment with Key Vault and Application Insights
- [ ] Event-driven layer: outbox pattern, Service Bus, and a notification worker with retries and dead-lettering

## License

This project is licensed under the [MIT License](LICENSE).
