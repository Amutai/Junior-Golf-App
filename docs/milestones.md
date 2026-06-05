# GitHub Milestones & Issues

Create these milestones and issues on GitHub after the initial commit.

---

## Milestone 1: Foundation
> Deadline: TBD | Priority: Critical

### Issues to Create

| # | Title | Labels |
|---|-------|--------|
| 1 | Set up Entity Framework Core with PostgreSQL | `enhancement`, `backend`, `database` |
| 2 | Configure ASP.NET Core Identity + JWT authentication | `enhancement`, `backend`, `auth` |
| 3 | Add health checks and API versioning | `enhancement`, `backend` |
| 4 | Set up Redis caching service | `enhancement`, `backend`, `infrastructure` |
| 5 | Add CodeQL and security scanning workflow | `enhancement`, `ci/cd` |
| 6 | Configure EF Core migrations pipeline | `enhancement`, `database`, `ci/cd` |
| 7 | Add global error handling and logging (Serilog) | `enhancement`, `backend` |
| 8 | Set up development environment documentation | `documentation` |

---

## Milestone 2: Core Membership
> Deadline: TBD | Priority: High

### Issues to Create

| # | Title | Labels |
|---|-------|--------|
| 9 | Member registration and profile CRUD | `enhancement`, `backend`, `feature` |
| 10 | M-PESA payment integration (Daraja API) | `enhancement`, `backend`, `payments` |
| 11 | Stripe payment integration | `enhancement`, `backend`, `payments` |
| 12 | Membership renewal and expiry logic | `enhancement`, `backend`, `feature` |
| 13 | QR code generation for member cards | `enhancement`, `backend`, `feature` |
| 14 | QR code scanning/verification endpoint | `enhancement`, `backend`, `feature` |
| 15 | Email notification service (SendGrid/SES) | `enhancement`, `backend`, `notifications` |
| 16 | SMS notification service (Africa's Talking) | `enhancement`, `backend`, `notifications` |

---

## Milestone 3: All-Platform App
> Deadline: TBD | Priority: High

### Issues to Create

| # | Title | Labels |
|---|-------|--------|
| 17 | Shared Razor component library (UI kit) | `enhancement`, `frontend`, `ui` |
| 18 | Authentication flow UI (login, register, 2FA) | `enhancement`, `frontend`, `auth` |
| 19 | Member dashboard and profile page | `enhancement`, `frontend`, `feature` |
| 20 | Payment flow UI (M-PESA + Stripe) | `enhancement`, `frontend`, `payments` |
| 21 | QR code display and scanner (MAUI camera) | `enhancement`, `mobile`, `feature` |
| 22 | Push notifications (MAUI + Firebase/APNs) | `enhancement`, `mobile`, `notifications` |
| 23 | Blazor WASM deployment configuration | `enhancement`, `frontend`, `infrastructure` |
| 24 | MAUI app store build configuration (Android/iOS) | `enhancement`, `mobile`, `ci/cd` |

---

## Milestone 4: Admin & Events
> Deadline: TBD | Priority: Medium

### Issues to Create

| # | Title | Labels |
|---|-------|--------|
| 25 | Admin dashboard — member management | `enhancement`, `admin`, `feature` |
| 26 | Event/tournament CRUD | `enhancement`, `backend`, `feature` |
| 27 | Tournament registration flow | `enhancement`, `frontend`, `feature` |
| 28 | Attendance tracking and reporting | `enhancement`, `admin`, `feature` |
| 29 | Revenue and membership analytics | `enhancement`, `admin`, `feature` |
| 30 | Role-based access control (Admin, Coach, Member, Guardian) | `enhancement`, `backend`, `auth` |

---

## Milestone 5: AI Integration
> Deadline: TBD | Priority: Medium

### Issues to Create

| # | Title | Labels |
|---|-------|--------|
| 31 | Semantic Kernel setup and configuration | `enhancement`, `ai`, `infrastructure` |
| 32 | FAQ chatbot (membership questions, Swahili/English) | `enhancement`, `ai`, `feature` |
| 33 | Player performance analytics and insights | `enhancement`, `ai`, `feature` |
| 34 | Smart event recommendations | `enhancement`, `ai`, `feature` |
| 35 | Churn prediction (membership renewal likelihood) | `enhancement`, `ai`, `feature` |
| 36 | Admin reporting copilot (natural language queries) | `enhancement`, `ai`, `admin` |
| 37 | OCR document verification for onboarding | `enhancement`, `ai`, `feature` |

---

## Milestone 6: Scale & Polish
> Deadline: TBD | Priority: Low (until user growth)

### Issues to Create

| # | Title | Labels |
|---|-------|--------|
| 38 | Load testing with k6/NBomber (1000+ concurrent) | `enhancement`, `testing`, `performance` |
| 39 | API response caching strategy | `enhancement`, `backend`, `performance` |
| 40 | Database query optimization and indexing | `enhancement`, `database`, `performance` |
| 41 | OpenTelemetry observability setup | `enhancement`, `infrastructure`, `monitoring` |
| 42 | Security hardening (rate limiting, CORS, CSP) | `enhancement`, `backend`, `security` |
| 43 | CI/CD pipeline for multi-platform releases | `enhancement`, `ci/cd`, `infrastructure` |

---

## Labels to Create

```
backend, frontend, mobile, admin, ai, database, auth, payments,
notifications, infrastructure, ci/cd, ui, feature, testing,
performance, monitoring, security, documentation
```
