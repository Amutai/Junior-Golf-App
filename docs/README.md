# Technical Documentation

This directory contains comprehensive technical documentation for the Junior Golf App - a modular monolithic application for managing junior golf memberships in Kenya.

## Quick Navigation

### 🏗️ Architecture & Design
- [System Architecture](./architecture.md) - Modular monolithic architecture overview
- [Database Schema](./database.md) - PostgreSQL schema and relationships
- [API Documentation](./api.md) - RESTful API endpoints and specifications

### 🚀 Development
- [Getting Started](./getting-started.md) - Setup guide for new developers
- [Development Workflow](./workflow.md) - Git workflow, PR process, and code standards
- [Branch Protection Strategy](./branch-protection-strategy.md) - Repository security and workflow configuration
- [Testing Strategy](./testing.md) - Unit, integration, and E2E testing approaches

### 🔧 Operations
- [Environment Setup](./environment.md) - Configuration for dev/staging/production
- [Deployment Guide](./deployment.md) - Docker, Kubernetes, and CI/CD procedures
- [Monitoring & Observability](./monitoring.md) - Metrics, logging, and alerting

## Project Structure

The Junior Golf App follows a **modular monolithic architecture**:

```
Junior-Golf-App/
├── apps/                    # Client applications
│   ├── web/                # Next.js web application
│   ├── mobile/             # React Native mobile app
│   └── admin/              # Admin dashboard
├── services/               # Backend services
│   ├── api/               # Express.js API server
│   └── database/          # Database schemas & migrations
├── packages/              # Shared packages
│   ├── shared/           # Common types & utilities
│   ├── ui/               # Reusable UI components
│   ├── auth/             # Authentication logic
│   ├── payments/         # Payment processing
│   └── notifications/    # Email/SMS notifications
└── docs/                 # This documentation
```

## Tech Stack

- **Frontend**: Next.js, React Native, TypeScript
- **Backend**: Node.js, Express.js, Prisma ORM
- **Database**: PostgreSQL with Redis caching
- **Authentication**: JWT with bcrypt
- **Payments**: Stripe, M-PESA API
- **Infrastructure**: Docker, Kubernetes, GitHub Actions

## Key Features

- **Multi-platform**: Web, mobile, and admin interfaces
- **Secure Authentication**: JWT-based with 2FA support
- **Payment Integration**: M-PESA and Stripe support
- **QR/NFC Verification**: Club entry verification
- **Event Management**: Tournament registration
- **Real-time Notifications**: Push and email notifications

## Development Quick Start

```bash
# Clone and setup
git clone <repository-url>
cd Junior-Golf-App
npm install

# Start development environment
docker-compose up -d
npm run dev

# Access applications
# Web App: http://localhost:3000
# API: http://localhost:3001
# Admin: http://localhost:3002
```

## Project Management Documentation

Organizational and process documents are maintained separately:
- **Project Management**: `../junior-golf-app-docs/project-management/`
- **Team Processes**: `../junior-golf-app-docs/processes/`
- **Templates**: `../junior-golf-app-docs/templates/`

See [../junior-golf-app-docs/README.md](../junior-golf-app-docs/README.md) for complete project management documentation.

## Contributing

1. Read the [Development Workflow](./workflow.md)
2. Follow the [Getting Started](./getting-started.md) guide
3. Review [Testing Strategy](./testing.md) before submitting PRs
4. Ensure all CI checks pass

## Support

For technical questions:
1. Check relevant documentation sections
2. Search existing GitHub issues
3. Contact the development team
4. Create a new issue if needed

---

**Last Updated**: December 2024  
**Version**: 1.0.0