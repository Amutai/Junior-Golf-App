# Database Schema

## Overview

The Junior Golf App uses PostgreSQL as the primary database with Prisma as the ORM for type-safe database operations.

## Database Technology Stack

- **Primary Database**: PostgreSQL 15+
- **ORM**: Prisma
- **Caching**: Redis
- **Migrations**: Prisma Migrate
- **Connection Pooling**: Built-in Prisma connection pooling

## Core Entities

### Users & Authentication

#### Users Table
```sql
- id: UUID (Primary Key)
- email: String (Unique)
- password_hash: String
- first_name: String
- last_name: String
- phone: String
- date_of_birth: Date
- created_at: DateTime
- updated_at: DateTime
- is_active: Boolean
- email_verified: Boolean
```

#### User Roles
```sql
- id: UUID (Primary Key)
- user_id: UUID (Foreign Key → Users)
- role: Enum (MEMBER, ADMIN, COACH, PARENT)
- created_at: DateTime
```

### Membership Management

#### Memberships
```sql
- id: UUID (Primary Key)
- user_id: UUID (Foreign Key → Users)
- membership_type: Enum (JUNIOR, STUDENT, PREMIUM)
- status: Enum (ACTIVE, SUSPENDED, EXPIRED)
- start_date: Date
- end_date: Date
- payment_status: Enum (PAID, PENDING, OVERDUE)
```

#### Member Profiles
```sql
- id: UUID (Primary Key)
- user_id: UUID (Foreign Key → Users)
- handicap: Decimal
- club_preferences: JSON
- emergency_contact: JSON
- medical_info: JSON (Encrypted)
```

### Events & Tournaments

#### Events
```sql
- id: UUID (Primary Key)
- title: String
- description: Text
- event_type: Enum (TOURNAMENT, TRAINING, SOCIAL)
- start_date: DateTime
- end_date: DateTime
- location: String
- max_participants: Integer
- registration_fee: Decimal
- status: Enum (DRAFT, OPEN, CLOSED, COMPLETED)
```

#### Event Registrations
```sql
- id: UUID (Primary Key)
- event_id: UUID (Foreign Key → Events)
- user_id: UUID (Foreign Key → Users)
- registration_date: DateTime
- payment_status: Enum (PAID, PENDING, REFUNDED)
- attendance_status: Enum (REGISTERED, ATTENDED, NO_SHOW)
```

### Payments

#### Transactions
```sql
- id: UUID (Primary Key)
- user_id: UUID (Foreign Key → Users)
- amount: Decimal
- currency: String (Default: KES)
- payment_method: Enum (MPESA, STRIPE, CASH)
- transaction_type: Enum (MEMBERSHIP, EVENT, MERCHANDISE)
- status: Enum (PENDING, COMPLETED, FAILED, REFUNDED)
- external_transaction_id: String
- created_at: DateTime
```

### Access Control

#### Club Access Logs
```sql
- id: UUID (Primary Key)
- user_id: UUID (Foreign Key → Users)
- access_method: Enum (QR_CODE, NFC, MANUAL)
- entry_time: DateTime
- exit_time: DateTime (Nullable)
- location: String
- verified_by: UUID (Foreign Key → Users, Nullable)
```

## Relationships

### One-to-Many
- Users → Memberships
- Users → Event Registrations
- Users → Transactions
- Events → Event Registrations

### Many-to-Many
- Users ↔ Events (through Event Registrations)
- Users ↔ Roles (through User Roles)

## Indexes

### Performance Indexes
```sql
-- User lookups
CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_phone ON users(phone);

-- Membership queries
CREATE INDEX idx_memberships_user_status ON memberships(user_id, status);
CREATE INDEX idx_memberships_expiry ON memberships(end_date) WHERE status = 'ACTIVE';

-- Event queries
CREATE INDEX idx_events_date_status ON events(start_date, status);
CREATE INDEX idx_event_registrations_event ON event_registrations(event_id);

-- Transaction queries
CREATE INDEX idx_transactions_user_date ON transactions(user_id, created_at);
CREATE INDEX idx_transactions_status ON transactions(status);

-- Access logs
CREATE INDEX idx_access_logs_user_time ON club_access_logs(user_id, entry_time);
```

## Data Migration Strategy

### Prisma Migrations
- All schema changes managed through Prisma Migrate
- Migration files stored in `/services/database/prisma/migrations/`
- Automatic migration deployment in CI/CD pipeline

### Data Seeding
- Development seed data in `/services/database/prisma/seed.ts`
- Production data imports through controlled scripts

## Backup & Recovery

### Backup Strategy
- Daily automated backups to AWS S3
- Point-in-time recovery capability
- Cross-region backup replication

### Recovery Procedures
- RTO (Recovery Time Objective): 4 hours
- RPO (Recovery Point Objective): 1 hour
- Automated failover for high availability

## Security Considerations

### Data Protection
- Sensitive data encryption at rest
- PII data anonymization in non-production environments
- Regular security audits and vulnerability assessments

### Access Control
- Database user roles with minimal privileges
- Connection encryption (SSL/TLS)
- IP whitelisting for database access
- Audit logging for all database operations