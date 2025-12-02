# API Service

Express.js REST API server for Junior Golfers Kenya membership system.

## Features

- RESTful API endpoints
- JWT authentication
- Database integration with Prisma
- Rate limiting and security middleware
- Payment webhook handling
- Real-time notifications

## Development

```bash
# Install dependencies
npm install

# Start development server
npm run dev

# Run database migrations
npm run db:migrate

# Generate Prisma client
npm run db:generate
```

## Environment Variables

```env
DATABASE_URL=postgresql://user:password@localhost:5432/junior_golf
JWT_SECRET=your-jwt-secret
STRIPE_SECRET_KEY=sk_test_...
MPESA_CONSUMER_KEY=your-mpesa-key
REDIS_URL=redis://localhost:6379
```

## API Documentation

API documentation is available at `/api/docs` when running in development mode.