# System Architecture

## Overview

Modular monolith built in C# / .NET 10. Single codebase produces:
- REST API (ASP.NET Core)
- Web app (Blazor WASM)
- Mobile/desktop app (MAUI Blazor Hybrid — Android, iOS, Windows, macOS)
- Admin dashboard (Blazor Server)

## Dependency Graph

```
JuniorGolf.App (MAUI) ──┐
JuniorGolf.Web (WASM) ──┼── JuniorGolf.Shared (DTOs, Razor components)
JuniorGolf.Admin ────────┤
                         │
JuniorGolf.Api ──────────┼── JuniorGolf.Core (Entities, Interfaces)
                         │         ↑
                         ├── JuniorGolf.Infrastructure (EF Core, Services)
                         └── JuniorGolf.AI (Semantic Kernel, ML.NET)
```

## Scaling Strategy

| Users | Infrastructure |
|-------|---------------|
| 1–1,000 | Single API instance, single PostgreSQL, Redis |
| 1,000–10,000 | Multiple API instances behind load balancer, Redis cluster, PG read replicas |
| 10,000+ | Add message queue (SQS/RabbitMQ), background workers, CDN |

## Key Patterns

- **Repository pattern** — abstracted data access via `IRepository<T>`
- **CQRS-lite** — separate read/write models where beneficial
- **Dependency injection** — built-in .NET DI container
- **Shared Razor components** — write once, render on all platforms via Blazor

## AI Architecture

- **Semantic Kernel** orchestrates LLM calls (Azure OpenAI / AWS Bedrock)
- **ML.NET** for lightweight on-server predictions (churn, recommendations)
- AI services injected via DI, easily swappable providers
