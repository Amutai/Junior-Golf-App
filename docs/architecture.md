# System Architecture

## Overview

Modular monolith built in C# / .NET 10. Single codebase produces:
- REST API (ASP.NET Core)
- Web app (Blazor WASM)
- Mobile/desktop app (MAUI Blazor Hybrid — Android, iOS, Windows, macOS)
- Admin dashboard (Blazor Server)

---

## System Flow Diagram

```
┌───────────────────────────────────────────────────────────────────────┐
│                          CLIENT LAYER                                 │
│                                                                       │
│  JuniorGolf.App (MAUI)  │  JuniorGolf.Web (WASM)  │  JuniorGolf.Admin │
│  [Android/iOS/Win/Mac]  │  [Browser]              │  [Server-side]    │
│                         │                         │                   │
│  All share: JuniorGolf.Shared (DTOs + Razor components)               │
│                                                                       │
└────────────────────────────────┬──────────────────────────────────────┘
                                 │
                          HTTP (JSON + JWT Bearer)
                                 │
                                 ▼
┌─────────────────────────────────────────────────────────────────────┐
│                       JuniorGolf.Api                                │
│                    (ASP.NET Core Web API)                           │
│                                                                     │
│  Request → Middleware (Auth, Logging, Error Handling)               │
│         → Controllers (route + validate)                            │
│         → Services (business logic)                                 │
│         → Repositories (data access)                                │
│         → Response (DTO)                                            │
│                                                                     │
│  References:                                                        │
│    ├── JuniorGolf.Core (entities, interfaces, enums)                │
│    ├── JuniorGolf.Infrastructure (EF Core, Redis, payments, email)  │
│    ├── JuniorGolf.AI (Semantic Kernel, ML.NET)                      │
│    └── JuniorGolf.Shared (DTOs, validators)                         │
└────────────────────────────────┬────────────────────────────────────┘
                                 │
                                 ▼
┌─────────────────────────────────────────────────────────────────────┐
│                  JuniorGolf.Infrastructure                          │
│                                                                     │
│  AppDbContext (EF Core) ───────────→ PostgreSQL                     │
│  RedisCacheService ────────────────→ Redis                          │
│  MpesaPaymentService ─────────────→ Safaricom Daraja API            │
│  StripePaymentService ─────────────→ Stripe API                     │
│  EmailService ─────────────────────→ SendGrid / AWS SES             │
│  SmsService ───────────────────────→ Africa's Talking API           │
│                                                                     │
│  References:                                                        │
│    └── JuniorGolf.Core (implements interfaces defined there)        │
└─────────────────────────────────────────────────────────────────────┘
                                 │
                                 ▼
┌─────────────────────────────────────────────────────────────────────┐
│                      JuniorGolf.Core                                │
│                   (ZERO external dependencies)                      │
│                                                                     │
│  Entities:    BaseEntity, Member, Event, Payment, etc.              │
│  Interfaces:  IRepository<T>, IMemberService, IPaymentService, etc. │
│  Enums:       MembershipStatus, PaymentMethod, UserRole, etc.       │
│                                                                     │
│  Rule: This project NEVER references any other project.             │
│        It defines WHAT the system does, not HOW.                    │
└─────────────────────────────────────────────────────────────────────┘
```

---

## Dependency Rules

| Project | Can Reference | Cannot Reference |
|---------|--------------|-----------------|
| JuniorGolf.Core | Nothing (pure domain) | Everything else |
| JuniorGolf.Shared | Nothing (pure DTOs/components) | Everything else |
| JuniorGolf.Infrastructure | Core | Api, Web, App, Admin, AI |
| JuniorGolf.AI | Core | Api, Web, App, Admin, Infrastructure |
| JuniorGolf.Api | Core, Infrastructure, AI, Shared | Web, App, Admin |
| JuniorGolf.Web | Shared | Core, Infrastructure, AI, Api, Admin |
| JuniorGolf.App | Shared | Core, Infrastructure, AI, Api, Admin |
| JuniorGolf.Admin | Core, Infrastructure, Shared | Web, App, AI |

---

## Data Flow: Request Lifecycle

### Example: Member Registration

```
INPUT                        PROCESS                           OUTPUT
─────                        ───────                           ──────
POST /api/members    →  AuthMiddleware (skip, public)    →  201 Created
{                        │                                    {
  "firstName": "...",    ▼                                      "id": "guid",
  "lastName": "...",     MembersController.Register()           "status": "Pending"
  "email": "...",        │                                    }
  "dateOfBirth": "..."   ├─ Validate (FluentValidation)
}                        │   ├── FAIL → 400 Bad Request
                         │   └── PASS ▼
                         │
                         ├─ Check duplicate email
                         │   ├── EXISTS → 409 Conflict
                         │   └── OK ▼
                         │
                         ├─ Create Member entity
                         ├─ Save to PostgreSQL (via IRepository<Member>)
                         ├─ Generate QR code
                         ├─ Send welcome email (background)
                         └─ Return MemberDto
```

### Example: M-PESA Payment

```
INPUT                        PROCESS                           OUTPUT
─────                        ───────                           ──────
POST /api/payments/     →  AuthMiddleware (JWT)           →  202 Accepted
  mpesa                      │                                 { "checkoutId": "..." }
{                            ▼
  "phone": "254...",     PaymentsController.InitiateMpesa()
  "amount": 5000             │
}                            ├─ Validate auth + membership
                             ├─ Call Daraja STK Push
                             │   └─ Safaricom sends push to user's phone
                             ├─ Store pending transaction
                             └─ Return checkout ID
                                    │
                                    ▼ (async callback)
                             POST /api/payments/mpesa/callback
                             │
                             ├─ Verify callback signature
                             ├─ SUCCESS:
                             │   ├─ Update transaction → Completed
                             │   ├─ Activate membership
                             │   ├─ Send SMS receipt
                             │   └─ Send email receipt
                             └─ FAILURE:
                                 └─ Update transaction → Failed
```

---

## Scaling Strategy

| Users | Infrastructure |
|-------|---------------|
| 1–1,000 | Single API instance, single PostgreSQL, Redis |
| 1,000–10,000 | Multiple API instances behind load balancer, Redis cluster, PG read replicas |
| 10,000+ | Add message queue (SQS/RabbitMQ), background workers, CDN |

---

## Key Patterns

- **Repository pattern** — abstracted data access via `IRepository<T>`
- **CQRS-lite** — separate read/write DTOs where beneficial
- **Dependency injection** — built-in .NET DI container
- **Shared Razor components** — write once, render on all platforms via Blazor
- **Background services** — `IHostedService` for email, SMS, expiry checks
- **Options pattern** — strongly-typed configuration via `IOptions<T>`

---

## AI Architecture

```
┌─────────────────────────────────────────────┐
│             JuniorGolf.AI                   │
│                                             │
│  Semantic Kernel ──→ LLM Provider           │
│  │                   (Azure OpenAI /        │
│  │                    AWS Bedrock)          │
│  │                                          │
│  ├── ChatService (FAQ bot)                  │
│  ├── RecommendationService (events)         │
│  └── AnalyticsService (performance)         │
│                                             │
│  ML.NET ──→ On-server models                │
│  │                                          │
│  ├── ChurnPredictionModel                   │
│  └── HandicapPredictionModel                │
└─────────────────────────────────────────────┘
```

- **Semantic Kernel** orchestrates LLM calls — swappable providers via config
- **ML.NET** for lightweight predictions (no GPU, runs in-process)
- AI services injected via DI, behind interfaces defined in Core
