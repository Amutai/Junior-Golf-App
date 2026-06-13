# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- CodeQL security scanning and Dependabot configuration
- Redis distributed caching (`ICacheService`)
- Health checks and API versioning
- ASP.NET Core Identity + JWT authentication
- Entity Framework Core with PostgreSQL
- Solution scaffold (API, Web, Admin, MAUI App, Core, Infrastructure, AI, Shared)
- CI/CD pipeline with GitHub Actions (build, test, MAUI build, CodeQL)
- Docker Compose for local development (PostgreSQL + Redis + API)
- Security policy, branch protection, secret scanning, push protection

### Infrastructure
- .NET 10 modular monolith architecture
- PostgreSQL database with Redis caching
- Secure credential management via environment variables
- Automated dependency updates via Dependabot

## [1.0.0] - TBD (Milestone 2: Core Membership)

### Planned
- Member CRUD operations
- M-PESA and Stripe payment processing
- QR code generation and verification
- Event management
- Push notifications
