# Junior Golfers Kenya Membership App

A modular monolithic client-server application for managing junior golf memberships in Kenya.

## Architecture

This project uses a **modular monolithic architecture** optimized for 1000+ users with horizontal scaling capabilities.

### Structure

```
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
├── docs/                 # Documentation
├── scripts/              # Build & deployment scripts
├── tools/                # Development tools
└── config/               # Configuration files
```

## Quick Start

```bash
# Install dependencies
npm install

# Start development servers
npm run dev

# Run tests
npm test

# Build all packages
npm run build
```

## Features

- **Multi-platform**: Web, mobile, and admin interfaces
- **Secure Authentication**: JWT-based with 2FA support
- **Payment Integration**: M-PESA and Stripe support
- **QR/NFC Verification**: Club entry verification
- **Event Management**: Tournament registration
- **Real-time Notifications**: Push and email notifications

## Tech Stack

- **Frontend**: Next.js, React Native, TypeScript
- **Backend**: Node.js, Express.js, Prisma
- **Database**: PostgreSQL with Redis caching
- **Authentication**: JWT with bcrypt
- **Payments**: Stripe, M-PESA API
- **Infrastructure**: Docker, GitHub Actions

## Development

See individual package READMEs for specific setup instructions:
- [Web App](./apps/web/README.md)
- [Mobile App](./apps/mobile/README.md)
- [API Service](./services/api/README.md)

## Contributing

1. Fork the repository
2. Create a feature branch
3. Follow conventional commits
4. Ensure tests pass
5. Submit a pull request

## License

MIT License - see [LICENSE](./LICENSE) file.