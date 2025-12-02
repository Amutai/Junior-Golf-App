# Environment Setup

## Overview

This document covers environment configuration for the Junior Golf App across different deployment stages.

## Environment Types

### Development
- **Purpose**: Local development and testing
- **Database**: Local PostgreSQL instance
- **External Services**: Mock/sandbox APIs
- **Logging**: Console output with debug level

### Staging
- **Purpose**: Pre-production testing and QA
- **Database**: Staging PostgreSQL instance
- **External Services**: Sandbox/test APIs
- **Logging**: File-based with info level

### Production
- **Purpose**: Live application serving users
- **Database**: Production PostgreSQL cluster
- **External Services**: Live APIs
- **Logging**: Structured logging with error tracking

## Environment Variables

### Core Configuration

#### Application Settings
```bash
# Application
NODE_ENV=development|staging|production
PORT=3001
APP_NAME="Junior Golf App"
APP_VERSION="1.0.0"
BASE_URL=http://localhost:3001

# CORS Settings
CORS_ORIGIN=http://localhost:3000,http://localhost:3002
CORS_CREDENTIALS=true
```

#### Database Configuration
```bash
# PostgreSQL
DATABASE_URL="postgresql://username:password@host:port/database"
DATABASE_POOL_SIZE=10
DATABASE_TIMEOUT=30000

# Redis (Caching & Sessions)
REDIS_URL="redis://localhost:6379"
REDIS_PASSWORD=""
REDIS_DB=0
```

#### Authentication & Security
```bash
# JWT Configuration
JWT_SECRET="your-super-secret-jwt-key-min-32-chars"
JWT_EXPIRES_IN="7d"
JWT_REFRESH_EXPIRES_IN="30d"

# Password Hashing
BCRYPT_ROUNDS=12

# Session Configuration
SESSION_SECRET="your-session-secret-key"
SESSION_TIMEOUT=3600000
```

#### Payment Integration
```bash
# Stripe
STRIPE_SECRET_KEY="sk_test_..."
STRIPE_PUBLISHABLE_KEY="pk_test_..."
STRIPE_WEBHOOK_SECRET="whsec_..."

# M-PESA
MPESA_CONSUMER_KEY="your-mpesa-consumer-key"
MPESA_CONSUMER_SECRET="your-mpesa-consumer-secret"
MPESA_PASSKEY="your-mpesa-passkey"
MPESA_SHORTCODE="174379"
MPESA_ENVIRONMENT="sandbox|production"
MPESA_CALLBACK_URL="https://yourdomain.com/api/webhooks/mpesa"
```

#### Email & SMS
```bash
# Email (SMTP)
SMTP_HOST="smtp.gmail.com"
SMTP_PORT=587
SMTP_SECURE=false
SMTP_USER="your-email@gmail.com"
SMTP_PASS="your-app-password"
EMAIL_FROM="Junior Golf App <noreply@juniorgolf.ke>"

# SMS Service
SMS_API_KEY="your-sms-api-key"
SMS_API_URL="https://api.africastalking.com/version1"
SMS_USERNAME="sandbox"
SMS_SENDER_ID="JuniorGolf"
```

#### File Storage
```bash
# AWS S3
AWS_ACCESS_KEY_ID="your-access-key"
AWS_SECRET_ACCESS_KEY="your-secret-key"
AWS_REGION="us-east-1"
AWS_S3_BUCKET="junior-golf-uploads"

# Local Storage (Development)
UPLOAD_PATH="./uploads"
MAX_FILE_SIZE=5242880
```

#### Monitoring & Logging
```bash
# Logging
LOG_LEVEL="debug|info|warn|error"
LOG_FORMAT="json|simple"

# Error Tracking (Sentry)
SENTRY_DSN="https://your-sentry-dsn"
SENTRY_ENVIRONMENT="development|staging|production"

# Analytics
GOOGLE_ANALYTICS_ID="GA-XXXXXXXXX"
```

## Environment-Specific Configurations

### Development (.env.local)
```bash
NODE_ENV=development
PORT=3001
BASE_URL=http://localhost:3001

# Database
DATABASE_URL="postgresql://postgres:password@localhost:5432/junior_golf_dev"
REDIS_URL="redis://localhost:6379"

# Authentication
JWT_SECRET="dev-jwt-secret-key-for-local-development-only"
JWT_EXPIRES_IN="24h"

# Payments (Test Mode)
STRIPE_SECRET_KEY="sk_test_51..."
STRIPE_PUBLISHABLE_KEY="pk_test_51..."
MPESA_ENVIRONMENT="sandbox"

# Email (Development)
SMTP_HOST="localhost"
SMTP_PORT=1025
EMAIL_FROM="dev@localhost"

# Logging
LOG_LEVEL="debug"
LOG_FORMAT="simple"

# File Storage
UPLOAD_PATH="./uploads"
```

### Staging (.env.staging)
```bash
NODE_ENV=staging
PORT=3001
BASE_URL=https://api-staging.juniorgolf.ke

# Database
DATABASE_URL="postgresql://user:pass@staging-db:5432/junior_golf_staging"
REDIS_URL="redis://staging-redis:6379"

# Authentication
JWT_SECRET="${STAGING_JWT_SECRET}"
JWT_EXPIRES_IN="7d"

# Payments (Test Mode)
STRIPE_SECRET_KEY="${STAGING_STRIPE_SECRET}"
STRIPE_PUBLISHABLE_KEY="${STAGING_STRIPE_PUBLIC}"
MPESA_ENVIRONMENT="sandbox"

# Email
SMTP_HOST="smtp.gmail.com"
SMTP_PORT=587
SMTP_USER="${STAGING_EMAIL_USER}"
SMTP_PASS="${STAGING_EMAIL_PASS}"

# Logging
LOG_LEVEL="info"
LOG_FORMAT="json"
SENTRY_DSN="${STAGING_SENTRY_DSN}"

# File Storage
AWS_S3_BUCKET="junior-golf-staging"
```

### Production (.env.production)
```bash
NODE_ENV=production
PORT=3001
BASE_URL=https://api.juniorgolf.ke

# Database
DATABASE_URL="${PRODUCTION_DATABASE_URL}"
REDIS_URL="${PRODUCTION_REDIS_URL}"

# Authentication
JWT_SECRET="${PRODUCTION_JWT_SECRET}"
JWT_EXPIRES_IN="7d"

# Payments (Live Mode)
STRIPE_SECRET_KEY="${PRODUCTION_STRIPE_SECRET}"
STRIPE_PUBLISHABLE_KEY="${PRODUCTION_STRIPE_PUBLIC}"
MPESA_ENVIRONMENT="production"

# Email
SMTP_HOST="smtp.gmail.com"
SMTP_PORT=587
SMTP_USER="${PRODUCTION_EMAIL_USER}"
SMTP_PASS="${PRODUCTION_EMAIL_PASS}"

# Logging
LOG_LEVEL="warn"
LOG_FORMAT="json"
SENTRY_DSN="${PRODUCTION_SENTRY_DSN}"

# File Storage
AWS_S3_BUCKET="junior-golf-production"
```

## Docker Configuration

### Development (docker-compose.yml)
```yaml
version: '3.8'

services:
  postgres:
    image: postgres:15
    environment:
      POSTGRES_DB: junior_golf_dev
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: password
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
    volumes:
      - redis_data:/data

  mailhog:
    image: mailhog/mailhog
    ports:
      - "1025:1025"
      - "8025:8025"

volumes:
  postgres_data:
  redis_data:
```

### Production (docker-compose.prod.yml)
```yaml
version: '3.8'

services:
  api:
    build:
      context: ./services/api
      dockerfile: Dockerfile.prod
    environment:
      - NODE_ENV=production
    env_file:
      - .env.production
    ports:
      - "3001:3001"
    depends_on:
      - postgres
      - redis
    restart: unless-stopped

  web:
    build:
      context: ./apps/web
      dockerfile: Dockerfile.prod
    ports:
      - "3000:3000"
    restart: unless-stopped

  nginx:
    image: nginx:alpine
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./nginx.conf:/etc/nginx/nginx.conf
      - ./ssl:/etc/nginx/ssl
    depends_on:
      - web
      - api
    restart: unless-stopped
```

## Infrastructure Setup

### AWS Configuration

#### RDS (PostgreSQL)
```bash
# Database Instance
Instance Class: db.t3.medium
Engine: PostgreSQL 15
Multi-AZ: Yes (Production)
Storage: 100GB SSD
Backup Retention: 7 days
```

#### ElastiCache (Redis)
```bash
# Cache Cluster
Node Type: cache.t3.micro
Engine: Redis 7.0
Cluster Mode: Disabled
Backup: Enabled
```

#### S3 Bucket
```bash
# File Storage
Bucket Name: junior-golf-{environment}
Region: us-east-1
Versioning: Enabled
Encryption: AES-256
```

### Kubernetes Configuration

#### Namespace
```yaml
apiVersion: v1
kind: Namespace
metadata:
  name: junior-golf-app
```

#### ConfigMap
```yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: app-config
  namespace: junior-golf-app
data:
  NODE_ENV: "production"
  PORT: "3001"
  LOG_LEVEL: "info"
```

#### Secret
```yaml
apiVersion: v1
kind: Secret
metadata:
  name: app-secrets
  namespace: junior-golf-app
type: Opaque
data:
  DATABASE_URL: <base64-encoded-url>
  JWT_SECRET: <base64-encoded-secret>
  STRIPE_SECRET_KEY: <base64-encoded-key>
```

## Security Considerations

### Environment Variable Security
- Never commit `.env` files to version control
- Use secret management systems in production
- Rotate secrets regularly
- Use different secrets for each environment

### Access Control
- Limit database access by IP
- Use IAM roles for AWS services
- Implement least privilege principle
- Regular security audits

### SSL/TLS Configuration
```nginx
# nginx.conf
server {
    listen 443 ssl http2;
    server_name api.juniorgolf.ke;
    
    ssl_certificate /etc/nginx/ssl/cert.pem;
    ssl_certificate_key /etc/nginx/ssl/key.pem;
    
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers ECDHE-RSA-AES256-GCM-SHA512:DHE-RSA-AES256-GCM-SHA512;
    ssl_prefer_server_ciphers off;
    
    location / {
        proxy_pass http://api:3001;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
    }
}
```

## Monitoring Setup

### Health Checks
```typescript
// health.ts
export const healthCheck = {
  database: async () => {
    try {
      await prisma.$queryRaw`SELECT 1`;
      return { status: 'healthy' };
    } catch (error) {
      return { status: 'unhealthy', error: error.message };
    }
  },
  
  redis: async () => {
    try {
      await redis.ping();
      return { status: 'healthy' };
    } catch (error) {
      return { status: 'unhealthy', error: error.message };
    }
  }
};
```

### Logging Configuration
```typescript
// logger.ts
import winston from 'winston';

export const logger = winston.createLogger({
  level: process.env.LOG_LEVEL || 'info',
  format: winston.format.combine(
    winston.format.timestamp(),
    winston.format.errors({ stack: true }),
    winston.format.json()
  ),
  transports: [
    new winston.transports.Console(),
    new winston.transports.File({ filename: 'app.log' })
  ]
});
```

## Troubleshooting

### Common Issues

#### Database Connection
```bash
# Check connection
psql -h localhost -p 5432 -U postgres -d junior_golf_dev

# Reset database
npm run db:reset --workspace=services/database
```

#### Redis Connection
```bash
# Test Redis
redis-cli ping

# Clear cache
redis-cli flushall
```

#### Environment Variables
```bash
# Check loaded variables
node -e "console.log(process.env)"

# Validate required variables
npm run validate-env
```