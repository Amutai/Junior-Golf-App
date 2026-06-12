# Junior Golfers Kenya

A cross-platform membership management application for junior golf in Kenya, built with .NET.

## Architecture

**Modular monolith** with shared code across all platforms — targeting 1000+ users with horizontal scaling.

### Solution Structure

```
├── src/
│   ├── JuniorGolf.Api/              # ASP.NET Core Web API
│   ├── JuniorGolf.Web/              # Blazor WebAssembly (browser)
│   ├── JuniorGolf.App/              # .NET MAUI Blazor Hybrid (Android/iOS/Windows/macOS)
│   ├── JuniorGolf.Admin/            # Blazor Server (admin dashboard)
│   ├── JuniorGolf.Core/             # Domain entities, interfaces, enums
│   ├── JuniorGolf.Infrastructure/   # EF Core, Redis, payments, notifications
│   ├── JuniorGolf.AI/               # Semantic Kernel, ML.NET
│   └── JuniorGolf.Shared/           # DTOs, shared Razor components
├── tests/
│   ├── JuniorGolf.Api.Tests/
│   ├── JuniorGolf.Core.Tests/
│   └── JuniorGolf.Infrastructure.Tests/
├── docs/
├── .github/
│   └── workflows/
├── docker-compose.yml
└── JuniorGolf.slnx
```

## Platforms

| Platform | Technology |
|----------|-----------|
| Android | .NET MAUI Blazor Hybrid |
| iOS | .NET MAUI Blazor Hybrid |
| Windows | .NET MAUI Blazor Hybrid |
| macOS | .NET MAUI (Mac Catalyst) |
| Web | Blazor WebAssembly |

## Tech Stack

- **Language**: C# / .NET 10
- **API**: ASP.NET Core Web API
- **ORM**: Entity Framework Core
- **Database**: PostgreSQL + Redis
- **Auth**: ASP.NET Core Identity + JWT
- **Payments**: M-PESA (Daraja API), Stripe
- **AI**: Microsoft Semantic Kernel
- **UI**: Blazor + .NET MAUI
- **Infrastructure**: Docker, GitHub Actions

## Quick Start

```bash
# Install required workloads
dotnet workload install wasm-tools maui

# Restore and build
dotnet restore
dotnet build

# Run the API
dotnet run --project src/JuniorGolf.Api

# Run the web app
dotnet run --project src/JuniorGolf.Web

# Run tests
dotnet test

# Docker (API + PostgreSQL + Redis)
docker-compose up
```

## Development Workflow

- **Branching**: `main` → `develop` → `feature/<issue>-description`
- **Commits**: Conventional commits (`feat:`, `fix:`, `docs:`)
- **PRs**: Reference GitHub Issues (`Closes #12`)
- **Milestones**: Track progress via GitHub Milestones

## Milestones

1. **M1: Foundation** — Scaffold, CI/CD, database, auth
2. **M2: Core Membership** — Member CRUD, payments, QR verification
3. **M3: All-Platform App** — MAUI Blazor Hybrid, shared components
4. **M4: Admin & Events** — Dashboard, tournament management
5. **M5: AI Integration** — Chatbot, recommendations, analytics
6. **M6: Scale & Polish** — Performance, security, monitoring

## Contributing

1. Pick an open GitHub Issue
2. Create a feature branch from `develop`
3. Follow conventional commits
4. Ensure `dotnet test` passes
5. Open a PR referencing the issue

## License

MIT License - see [LICENSE](./LICENSE) file.
