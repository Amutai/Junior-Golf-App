# Web Application

Next.js web application for Junior Golfers Kenya membership management.

## Features

- User registration and profile management
- Membership subscription and renewal
- QR code generation for club entry
- Event browsing and registration
- Payment processing

## Development

```bash
# Install dependencies
npm install

# Start development server
npm run dev

# Build for production
npm run build

# Run tests
npm test
```

## Environment Variables

```env
NEXT_PUBLIC_API_URL=http://localhost:8000
NEXT_PUBLIC_STRIPE_PUBLISHABLE_KEY=pk_test_...
```

## Deployment

The app is deployed automatically via GitHub Actions on push to main branch.