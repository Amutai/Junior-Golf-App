# Testing Strategy

## Overview

The Junior Golf App implements a comprehensive testing strategy covering unit, integration, and end-to-end testing across all applications and services.

## Testing Philosophy

### Principles
- **Test Pyramid**: More unit tests, fewer integration tests, minimal E2E tests
- **Test-Driven Development**: Write tests before implementation when possible
- **Continuous Testing**: Tests run on every commit and PR
- **Quality Gates**: Minimum 80% code coverage required

### Testing Levels
1. **Unit Tests**: Individual functions and components
2. **Integration Tests**: API endpoints and database interactions
3. **End-to-End Tests**: Complete user workflows
4. **Performance Tests**: Load and stress testing

## Testing Stack

### Frontend Testing
- **Framework**: Jest + Testing Library
- **Component Testing**: React Testing Library
- **Mocking**: Jest mocks + MSW (Mock Service Worker)
- **Snapshot Testing**: Jest snapshots for UI components

### Backend Testing
- **Framework**: Jest + Supertest
- **Database Testing**: In-memory SQLite for unit tests
- **API Testing**: Supertest for HTTP endpoints
- **Mocking**: Jest mocks for external services

### E2E Testing
- **Framework**: Playwright
- **Browser Support**: Chrome, Firefox, Safari
- **Mobile Testing**: Device emulation
- **Visual Testing**: Screenshot comparisons

## Unit Testing

### Frontend Components

#### React Component Testing
```typescript
// Example: UserProfile.test.tsx
import { render, screen } from '@testing-library/react';
import { UserProfile } from './UserProfile';

describe('UserProfile', () => {
  it('displays user information correctly', () => {
    const mockUser = {
      firstName: 'John',
      lastName: 'Doe',
      email: 'john@example.com'
    };

    render(<UserProfile user={mockUser} />);
    
    expect(screen.getByText('John Doe')).toBeInTheDocument();
    expect(screen.getByText('john@example.com')).toBeInTheDocument();
  });

  it('handles loading state', () => {
    render(<UserProfile user={null} loading={true} />);
    
    expect(screen.getByTestId('loading-spinner')).toBeInTheDocument();
  });
});
```

#### Custom Hooks Testing
```typescript
// Example: useAuth.test.ts
import { renderHook, act } from '@testing-library/react';
import { useAuth } from './useAuth';

describe('useAuth', () => {
  it('should login user successfully', async () => {
    const { result } = renderHook(() => useAuth());

    await act(async () => {
      await result.current.login('user@example.com', 'password');
    });

    expect(result.current.user).toBeDefined();
    expect(result.current.isAuthenticated).toBe(true);
  });
});
```

### Backend Services

#### API Endpoint Testing
```typescript
// Example: auth.test.ts
import request from 'supertest';
import { app } from '../app';
import { prisma } from '../lib/prisma';

describe('POST /auth/login', () => {
  beforeEach(async () => {
    await prisma.user.deleteMany();
  });

  it('should login with valid credentials', async () => {
    // Create test user
    await prisma.user.create({
      data: {
        email: 'test@example.com',
        passwordHash: 'hashed_password'
      }
    });

    const response = await request(app)
      .post('/auth/login')
      .send({
        email: 'test@example.com',
        password: 'password'
      });

    expect(response.status).toBe(200);
    expect(response.body.token).toBeDefined();
  });

  it('should reject invalid credentials', async () => {
    const response = await request(app)
      .post('/auth/login')
      .send({
        email: 'invalid@example.com',
        password: 'wrongpassword'
      });

    expect(response.status).toBe(401);
    expect(response.body.error).toBeDefined();
  });
});
```

#### Service Layer Testing
```typescript
// Example: userService.test.ts
import { UserService } from './UserService';
import { prisma } from '../lib/prisma';

jest.mock('../lib/prisma');

describe('UserService', () => {
  const userService = new UserService();

  it('should create user successfully', async () => {
    const mockUser = {
      id: '1',
      email: 'test@example.com',
      firstName: 'John',
      lastName: 'Doe'
    };

    (prisma.user.create as jest.Mock).mockResolvedValue(mockUser);

    const result = await userService.createUser({
      email: 'test@example.com',
      firstName: 'John',
      lastName: 'Doe',
      password: 'password'
    });

    expect(result).toEqual(mockUser);
    expect(prisma.user.create).toHaveBeenCalledWith({
      data: expect.objectContaining({
        email: 'test@example.com',
        firstName: 'John',
        lastName: 'Doe'
      })
    });
  });
});
```

## Integration Testing

### Database Integration
```typescript
// Example: database.integration.test.ts
import { PrismaClient } from '@prisma/client';

const prisma = new PrismaClient({
  datasources: {
    db: {
      url: process.env.TEST_DATABASE_URL
    }
  }
});

describe('User Database Operations', () => {
  beforeAll(async () => {
    await prisma.$connect();
  });

  afterAll(async () => {
    await prisma.$disconnect();
  });

  beforeEach(async () => {
    await prisma.user.deleteMany();
  });

  it('should create and retrieve user', async () => {
    const userData = {
      email: 'test@example.com',
      firstName: 'John',
      lastName: 'Doe',
      passwordHash: 'hashed_password'
    };

    const createdUser = await prisma.user.create({
      data: userData
    });

    const retrievedUser = await prisma.user.findUnique({
      where: { id: createdUser.id }
    });

    expect(retrievedUser).toMatchObject(userData);
  });
});
```

### API Integration Testing
```typescript
// Example: api.integration.test.ts
import request from 'supertest';
import { app } from '../app';
import { setupTestDatabase, cleanupTestDatabase } from '../test/helpers';

describe('User API Integration', () => {
  beforeAll(async () => {
    await setupTestDatabase();
  });

  afterAll(async () => {
    await cleanupTestDatabase();
  });

  it('should complete user registration flow', async () => {
    // Register user
    const registerResponse = await request(app)
      .post('/auth/register')
      .send({
        email: 'test@example.com',
        password: 'password123',
        firstName: 'John',
        lastName: 'Doe'
      });

    expect(registerResponse.status).toBe(201);
    const { token } = registerResponse.body;

    // Get user profile
    const profileResponse = await request(app)
      .get('/users/profile')
      .set('Authorization', `Bearer ${token}`);

    expect(profileResponse.status).toBe(200);
    expect(profileResponse.body.email).toBe('test@example.com');
  });
});
```

## End-to-End Testing

### User Workflows
```typescript
// Example: user-registration.e2e.test.ts
import { test, expect } from '@playwright/test';

test.describe('User Registration Flow', () => {
  test('should register new user successfully', async ({ page }) => {
    await page.goto('/register');

    // Fill registration form
    await page.fill('[data-testid="email-input"]', 'test@example.com');
    await page.fill('[data-testid="password-input"]', 'password123');
    await page.fill('[data-testid="firstName-input"]', 'John');
    await page.fill('[data-testid="lastName-input"]', 'Doe');

    // Submit form
    await page.click('[data-testid="register-button"]');

    // Verify success
    await expect(page).toHaveURL('/dashboard');
    await expect(page.locator('[data-testid="welcome-message"]')).toContainText('Welcome, John');
  });

  test('should show validation errors for invalid input', async ({ page }) => {
    await page.goto('/register');

    // Submit empty form
    await page.click('[data-testid="register-button"]');

    // Verify validation errors
    await expect(page.locator('[data-testid="email-error"]')).toBeVisible();
    await expect(page.locator('[data-testid="password-error"]')).toBeVisible();
  });
});
```

### Mobile Testing
```typescript
// Example: mobile.e2e.test.ts
import { test, expect, devices } from '@playwright/test';

test.describe('Mobile User Experience', () => {
  test.use({ ...devices['iPhone 12'] });

  test('should navigate mobile menu', async ({ page }) => {
    await page.goto('/');

    // Open mobile menu
    await page.click('[data-testid="mobile-menu-button"]');
    await expect(page.locator('[data-testid="mobile-menu"]')).toBeVisible();

    // Navigate to events
    await page.click('[data-testid="events-menu-item"]');
    await expect(page).toHaveURL('/events');
  });
});
```

## Performance Testing

### Load Testing
```typescript
// Example: load-test.js (using k6)
import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
  stages: [
    { duration: '2m', target: 100 }, // Ramp up
    { duration: '5m', target: 100 }, // Stay at 100 users
    { duration: '2m', target: 0 },   // Ramp down
  ],
};

export default function () {
  let response = http.get('http://localhost:3001/api/events');
  
  check(response, {
    'status is 200': (r) => r.status === 200,
    'response time < 500ms': (r) => r.timings.duration < 500,
  });
  
  sleep(1);
}
```

## Test Configuration

### Jest Configuration
```javascript
// jest.config.js
module.exports = {
  preset: 'ts-jest',
  testEnvironment: 'jsdom',
  setupFilesAfterEnv: ['<rootDir>/src/test/setup.ts'],
  collectCoverageFrom: [
    'src/**/*.{ts,tsx}',
    '!src/**/*.d.ts',
    '!src/test/**/*',
  ],
  coverageThreshold: {
    global: {
      branches: 80,
      functions: 80,
      lines: 80,
      statements: 80,
    },
  },
  moduleNameMapping: {
    '^@/(.*)$': '<rootDir>/src/$1',
  },
};
```

### Playwright Configuration
```typescript
// playwright.config.ts
import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: './e2e',
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,
  reporter: 'html',
  use: {
    baseURL: 'http://localhost:3000',
    trace: 'on-first-retry',
  },
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
    {
      name: 'firefox',
      use: { ...devices['Desktop Firefox'] },
    },
    {
      name: 'webkit',
      use: { ...devices['Desktop Safari'] },
    },
    {
      name: 'Mobile Chrome',
      use: { ...devices['Pixel 5'] },
    },
  ],
  webServer: {
    command: 'npm run dev',
    url: 'http://localhost:3000',
    reuseExistingServer: !process.env.CI,
  },
});
```

## Test Data Management

### Test Fixtures
```typescript
// test/fixtures/users.ts
export const testUsers = {
  validUser: {
    email: 'john@example.com',
    firstName: 'John',
    lastName: 'Doe',
    password: 'password123'
  },
  adminUser: {
    email: 'admin@example.com',
    firstName: 'Admin',
    lastName: 'User',
    password: 'admin123',
    role: 'ADMIN'
  }
};
```

### Database Seeding
```typescript
// test/helpers/database.ts
export async function seedTestData() {
  await prisma.user.createMany({
    data: [
      testUsers.validUser,
      testUsers.adminUser
    ]
  });
}

export async function cleanupTestData() {
  await prisma.user.deleteMany();
}
```

## CI/CD Integration

### GitHub Actions
```yaml
# .github/workflows/test.yml
name: Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    
    services:
      postgres:
        image: postgres:15
        env:
          POSTGRES_PASSWORD: postgres
        options: >-
          --health-cmd pg_isready
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5
    
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-node@v4
        with:
          node-version: '18'
          cache: 'npm'
      
      - run: npm ci
      - run: npm run test:unit
      - run: npm run test:integration
      - run: npm run test:e2e
      
      - name: Upload coverage
        uses: codecov/codecov-action@v3
```

## Best Practices

### Test Organization
- Group related tests in describe blocks
- Use descriptive test names
- Follow AAA pattern (Arrange, Act, Assert)
- Keep tests independent and isolated

### Mocking Strategy
- Mock external dependencies
- Use real database for integration tests
- Mock time-dependent functions
- Avoid over-mocking

### Test Maintenance
- Update tests when requirements change
- Remove obsolete tests
- Refactor test code like production code
- Monitor test execution time