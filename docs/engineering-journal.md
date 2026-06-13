# Engineering Journal

A reverse-chronological decision log capturing the *why*, *where*, and *how* for each piece of work in the Junior Golf App. Updated with every issue.

---

## Issue #7: Set Up Redis Caching Service

**Milestone:** M1 Foundation  
**Status:** In Progress

### What & Why

Configure Redis as a distributed cache for frequently accessed data (member profiles, session tokens). This reduces database load and improves response times. At 1000+ users, repeatedly hitting PostgreSQL for the same member profile on every request is wasteful — Redis gives us sub-millisecond reads.

### Where It Fits

```
JuniorGolf.Api
    │
    │ Controller calls service
    ▼
JuniorGolf.Infrastructure
    │
    ├─ ICacheService.GetAsync<T>("member:123")
    │   ├── CACHE HIT → return cached data (fast, no DB call)
    │   └── CACHE MISS → fetch from PostgreSQL → store in Redis → return
    │
    ├─ AppDbContext → PostgreSQL (source of truth)
    └─ RedisCacheService → Redis (fast read layer)
```

### Data Flow

```
INPUT                        PROCESS                              OUTPUT
─────                        ───────                              ──────
GetAsync<T>(key)        →  Check Redis for key               →  T? (cached value or null)
                              ├── EXISTS → deserialize, return
                              └── NOT EXISTS → return null

SetAsync<T>(key, value) →  Serialize → store in Redis        →  void
                              with TTL (expiry)

RemoveAsync(key)        →  Delete key from Redis             →  void
                              (used on data update to invalidate)
```

### Key Decisions

- **Interface in Core, implementation in Infrastructure** — follows dependency inversion. Any service can request caching without knowing it's Redis.
- **TTL-based expiry** — avoids stale data. Default 5 minutes, configurable per call.
- **JSON serialization** — human-readable, debuggable via Redis CLI. Acceptable perf for our scale.
- **Cache-aside pattern** — caller decides when to cache/invalidate. Simple, predictable.

### Dependencies

- Docker Compose already has Redis 7 configured (port 6379)
- No dependency on other issues

---

## Issue #6: Add Health Checks and API Versioning

**Milestone:** M1 Foundation  
**Status:** ✅ Done

### What & Why

Production-readiness endpoints for load balancers and monitoring. Without health checks, a load balancer can't tell if an instance is alive but unable to serve (e.g., database connection lost). API versioning ensures we can evolve endpoints without breaking existing mobile app versions in the wild.

### Where It Fits

```
Load Balancer / Monitoring
    │
    │ GET /health        (liveness: is the process alive?)
    │ GET /health/ready  (readiness: can it serve traffic?)
    ▼
JuniorGolf.Api
    │ HealthCheck middleware
    ▼
PostgreSQL (connectivity test)
```

### Data Flow

```
GET /health       → Always 200 if app process is running (liveness)
GET /health/ready → Checks PostgreSQL → 200 Healthy or 503 Unhealthy (readiness)
```

### Key Decisions

- **Two endpoints** — liveness vs readiness is a Kubernetes/cloud best practice. Liveness triggers restart, readiness removes from rotation.
- **URL segment versioning** — `/api/v1/...` is most discoverable and cache-friendly. Also support `X-Api-Version` header as fallback.
- **global.json added** — pinned SDK version after discovering that deleting `obj/` folders caused the system to fall back to .NET 2.1 SDK.
- **Removed EF Core Design package** — it pulled in Mono.TextTemplating which is incompatible with .NET 10. The `dotnet-ef` global tool provides this at CLI time instead.

### Dependencies

- Issue #4 (EF Core) — needed for PostgreSQL health check
- Issue #5 (Identity) — health check verifies the full stack including identity tables

---

## Issue #5: Configure ASP.NET Core Identity + JWT Authentication

**Milestone:** M1 Foundation  
**Status:** ✅ Done

### What & Why

User management and stateless API authentication. Identity handles the hard parts (password hashing, lockout, 2FA foundation). JWT tokens make the API stateless — any instance can validate a token without session storage, which is critical for horizontal scaling.

### Where It Fits

```
Client (App/Web/Admin)
    │
    │ POST /api/auth/login {email, password}
    ▼
JuniorGolf.Api
    │ AuthController → IAuthService
    ▼
JuniorGolf.Infrastructure
    │ AuthService → UserManager (Identity) → AppDbContext → PostgreSQL
    │            → JwtSecurityTokenHandler → signed JWT
    ▼
Response: { token: "eyJ...", refreshToken: "..." }
    │
    │ Subsequent requests: Authorization: Bearer eyJ...
    ▼
JWT Middleware validates signature → grants/denies access
```

### Data Flow

```
Register: Input → validate → create user → assign role → generate JWT → return tokens
Login:    Input → find user → verify password hash → generate JWT → return tokens
```

### Key Decisions

- **ApplicationUser extends IdentityUser** — keeps Identity's battle-tested password hashing while adding our domain fields (FirstName, LastName, MemberId link).
- **IAuthService in Core** — the API depends on an abstraction, not Identity directly. Could swap to external auth provider later.
- **Roles seeded on startup** — Admin, Coach, Member, Guardian. Deterministic, no manual setup needed.
- **JWT in Infrastructure, not API** — token generation is an infrastructure concern, not a controller concern.
- **Refresh token placeholder** — generates a random token but doesn't persist it yet. Full implementation comes with Redis (Issue #7) for token storage.
- **Password policy** — 8+ chars, require digit, no special char requirement (reduces user friction for junior/parent audience).

### Dependencies

- Issue #4 (EF Core) — Identity stores users in PostgreSQL via AppDbContext

---

## Issue #4: Set Up Entity Framework Core with PostgreSQL

**Milestone:** M1 Foundation  
**Status:** ✅ Done

### What & Why

The data persistence foundation. Every feature in the app (members, payments, events) needs to read/write data. EF Core provides a strongly-typed, LINQ-based abstraction over SQL, with migrations for schema evolution.

### Where It Fits

```
JuniorGolf.Api (calls repositories via DI)
    │
    ▼
JuniorGolf.Infrastructure (implements IRepository<T>)
    │
    ├─ AppDbContext (EF Core) → maps entities to tables
    ▼
PostgreSQL (data storage)
```

### Data Flow

```
API Controller
  → calls IRepository<Member>.GetByIdAsync(guid)
    → Repository<Member> calls DbSet<Member>.FindAsync(guid)
      → EF Core generates SQL: SELECT * FROM "Members" WHERE "Id" = @p0
        → Npgsql sends to PostgreSQL
          → Returns row → EF maps to Member entity
            → Controller returns as DTO
```

### Key Decisions

- **Generic Repository pattern** — `IRepository<T>` in Core, `Repository<T>` in Infrastructure. Any entity gets CRUD for free. Avoids writing repetitive data access code.
- **Interface in Core, implementation in Infrastructure** — dependency inversion. Core has zero dependencies; Infrastructure knows about EF Core and Npgsql.
- **Auto-UpdatedAt in SaveChangesAsync** — every entity gets automatic timestamp tracking without manual code in every service.
- **Unique index on Member.Email** — enforced at database level, not just application code. Defense in depth.
- **String enum storage** — `MembershipStatus` stored as "Active", "Pending" etc. Human-readable in DB, avoids int-to-meaning guesswork.
- **Connection string from configuration** — supports appsettings for dev, environment variables for Docker/production.

### Dependencies

- None (this is the foundation everything else builds on)

---

## Architecture Baseline (Initial Scaffold)

**Milestone:** M1 Foundation  
**Status:** ✅ Done

### What & Why

Replaced the Node.js/TypeScript scaffold with a C#/.NET 10 modular monolith. Single language across all platforms (API, web, mobile, desktop). Chose .NET over Node.js for: type safety without a transpilation layer, superior performance under load, native cross-platform via MAUI, and unified tooling.

### Solution Structure Rationale

```
src/
├── JuniorGolf.Core/           # Pure domain. ZERO dependencies. Defines WHAT.
├── JuniorGolf.Infrastructure/ # Implements Core interfaces. Knows HOW.
├── JuniorGolf.Api/            # HTTP layer. Orchestrates. Thin.
├── JuniorGolf.Web/            # Blazor WASM. Shares components with App.
├── JuniorGolf.App/            # MAUI Blazor Hybrid. All platforms.
├── JuniorGolf.Admin/          # Blazor Server. Internal use.
├── JuniorGolf.AI/             # Semantic Kernel + ML.NET. Isolated.
└── JuniorGolf.Shared/         # DTOs + Razor components. Used by all clients.
```

### Key Decisions

- **Modular monolith over microservices** — at 1000 users, microservices add complexity without benefit. Can extract later if needed.
- **.NET MAUI Blazor Hybrid** — write Razor components once, render natively on Android/iOS/Windows/macOS AND in browser.
- **Shared project for DTOs + components** — eliminates duplication between web and mobile clients.
- **AI as separate project** — keeps ML dependencies isolated. Can be disabled entirely without affecting core functionality.
- **GitHub milestones + issue-driven development** — every commit traces to an issue, every issue traces to a milestone. Full traceability.
