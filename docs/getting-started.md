# Getting Started

## Prerequisites

### Required Software
- **Node.js**: Version 18+ (LTS recommended)
- **npm**: Version 8+ (comes with Node.js)
- **Docker**: Latest version
- **Docker Compose**: Version 2.0+
- **Git**: Latest version

### Development Tools (Recommended)
- **VS Code**: With recommended extensions
- **Postman**: For API testing
- **TablePlus/pgAdmin**: For database management

## Quick Setup

### 1. Clone the Repository
```bash
git clone https://github.com/your-org/junior-golf-app.git
cd junior-golf-app
```

### 2. Install Dependencies
```bash
# Install all workspace dependencies
npm install
```

### 3. Environment Setup
```bash
# Copy environment template
cp .env.example .env.local

# Edit environment variables
nano .env.local
```

### 4. Start Development Environment
```bash
# Start all services with Docker Compose
docker-compose up -d

# Start development servers
npm run dev
```

### 5. Verify Setup
- **Web App**: http://localhost:3000
- **API Service**: http://localhost:3001
- **Admin Dashboard**: http://localhost:3002
- **Database**: localhost:5432

## Environment Variables

### Required Variables
```bash
# Database
DATABASE_URL="postgresql://postgres:password@localhost:5432/junior_golf_dev"
REDIS_URL="redis://localhost:6379"

# Authentication
JWT_SECRET="your-super-secret-jwt-key"
JWT_EXPIRES_IN="7d"

# Payments
STRIPE_SECRET_KEY="sk_test_..."
STRIPE_PUBLISHABLE_KEY="pk_test_..."
MPESA_CONSUMER_KEY="your-mpesa-key"
MPESA_CONSUMER_SECRET="your-mpesa-secret"

# Email
SMTP_HOST="smtp.gmail.com"
SMTP_PORT=587
SMTP_USER="your-email@gmail.com"
SMTP_PASS="your-app-password"

# SMS
SMS_API_KEY="your-sms-api-key"
SMS_SENDER_ID="JuniorGolf"
```

## Development Workflow

### 1. Branch Strategy
```bash
# Create feature branch
git checkout -b feature/your-feature-name

# Work on your changes
git add .
git commit -m "feat: add new feature"

# Push and create PR
git push origin feature/your-feature-name
```

### 2. Running Tests
```bash
# Run all tests
npm test

# Run tests for specific workspace
npm run test --workspace=apps/web

# Run tests in watch mode
npm run test:watch
```

### 3. Code Quality
```bash
# Lint all code
npm run lint

# Fix linting issues
npm run lint:fix

# Type checking
npm run type-check

# Format code
npm run format
```

## Workspace Structure

### Applications (`/apps`)
```bash
# Start web app only
npm run dev --workspace=apps/web

# Start mobile app
npm run dev --workspace=apps/mobile

# Start admin dashboard
npm run dev --workspace=apps/admin
```

### Services (`/services`)
```bash
# Start API service only
npm run dev --workspace=services/api

# Run database migrations
npm run migrate --workspace=services/database

# Seed database
npm run seed --workspace=services/database
```

### Packages (`/packages`)
```bash
# Build shared packages
npm run build --workspace=packages/shared

# Watch for changes in UI package
npm run dev --workspace=packages/ui
```

## Database Setup

### 1. Start PostgreSQL
```bash
# Using Docker Compose
docker-compose up -d postgres

# Or install locally (macOS)
brew install postgresql
brew services start postgresql
```

### 2. Run Migrations
```bash
cd services/database
npm run migrate:dev
```

### 3. Seed Development Data
```bash
cd services/database
npm run seed
```

## Common Commands

### Development
```bash
# Start all services
npm run dev

# Start specific service
npm run dev --workspace=apps/web

# Build all packages
npm run build

# Clean all node_modules
npm run clean
```

### Database
```bash
# Reset database
npm run db:reset --workspace=services/database

# Generate Prisma client
npm run db:generate --workspace=services/database

# View database in Prisma Studio
npm run db:studio --workspace=services/database
```

### Testing
```bash
# Run unit tests
npm run test:unit

# Run integration tests
npm run test:integration

# Run e2e tests
npm run test:e2e

# Generate test coverage
npm run test:coverage
```

## Troubleshooting

### Common Issues

#### Port Already in Use
```bash
# Kill process on port 3000
lsof -ti:3000 | xargs kill -9

# Or use different port
PORT=3001 npm run dev
```

#### Database Connection Issues
```bash
# Check if PostgreSQL is running
docker-compose ps postgres

# Restart database
docker-compose restart postgres

# Check connection
psql -h localhost -p 5432 -U postgres -d junior_golf_dev
```

#### Node Modules Issues
```bash
# Clean install
rm -rf node_modules package-lock.json
npm install

# Clear npm cache
npm cache clean --force
```

### Getting Help

1. **Check Documentation**: Review relevant docs in `/docs`
2. **Search Issues**: Look for similar issues in GitHub
3. **Ask Team**: Use team Slack channel
4. **Create Issue**: If problem persists, create GitHub issue

## Next Steps

1. **Read Architecture**: [System Architecture](./architecture.md)
2. **API Documentation**: [API Reference](./api.md)
3. **Database Schema**: [Database Documentation](./database.md)
4. **Testing Guide**: [Testing Strategy](./testing.md)
5. **Deployment**: [Deployment Guide](./deployment.md)