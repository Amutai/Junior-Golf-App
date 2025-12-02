# API Documentation

## Overview

The Junior Golf App API is built with Node.js and Express.js, providing RESTful endpoints for all client applications.

## Base Configuration

- **Base URL**: `https://api.juniorgolf.ke` (Production)
- **Development URL**: `http://localhost:3001`
- **API Version**: v1
- **Content Type**: `application/json`
- **Authentication**: JWT Bearer tokens

## Authentication

### Endpoints

#### POST /auth/register
Register a new user account.

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "securePassword123",
  "firstName": "John",
  "lastName": "Doe",
  "phone": "+254700000000",
  "dateOfBirth": "2010-05-15"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "user": {
      "id": "uuid",
      "email": "user@example.com",
      "firstName": "John",
      "lastName": "Doe"
    },
    "token": "jwt_token_here"
  }
}
```

#### POST /auth/login
Authenticate user and receive access token.

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "securePassword123"
}
```

#### POST /auth/refresh
Refresh expired access token.

#### POST /auth/logout
Invalidate current session.

## User Management

### Endpoints

#### GET /users/profile
Get current user profile information.

**Headers:**
```
Authorization: Bearer <jwt_token>
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": "uuid",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "phone": "+254700000000",
    "membership": {
      "type": "JUNIOR",
      "status": "ACTIVE",
      "expiryDate": "2024-12-31"
    }
  }
}
```

#### PUT /users/profile
Update user profile information.

#### GET /users/membership
Get user membership details.

#### POST /users/membership/renew
Renew user membership.

## Events Management

### Endpoints

#### GET /events
Get list of upcoming events.

**Query Parameters:**
- `page`: Page number (default: 1)
- `limit`: Items per page (default: 10)
- `type`: Event type filter (TOURNAMENT, TRAINING, SOCIAL)
- `status`: Event status filter (OPEN, CLOSED)

**Response:**
```json
{
  "success": true,
  "data": {
    "events": [
      {
        "id": "uuid",
        "title": "Junior Championship 2024",
        "description": "Annual junior golf championship",
        "eventType": "TOURNAMENT",
        "startDate": "2024-06-15T09:00:00Z",
        "endDate": "2024-06-15T17:00:00Z",
        "location": "Karen Country Club",
        "maxParticipants": 50,
        "registrationFee": 2000,
        "status": "OPEN"
      }
    ],
    "pagination": {
      "page": 1,
      "limit": 10,
      "total": 25,
      "pages": 3
    }
  }
}
```

#### GET /events/:id
Get specific event details.

#### POST /events/:id/register
Register for an event.

#### DELETE /events/:id/register
Cancel event registration.

## Payments

### Endpoints

#### POST /payments/mpesa/initiate
Initiate M-PESA payment.

**Request Body:**
```json
{
  "amount": 2000,
  "phoneNumber": "+254700000000",
  "transactionType": "MEMBERSHIP",
  "description": "Membership renewal"
}
```

#### POST /payments/stripe/create-intent
Create Stripe payment intent.

#### GET /payments/transactions
Get user transaction history.

#### GET /payments/transactions/:id
Get specific transaction details.

## Club Access

### Endpoints

#### POST /access/verify-qr
Verify QR code for club entry.

**Request Body:**
```json
{
  "qrCode": "encrypted_qr_data",
  "location": "Main Entrance"
}
```

#### POST /access/verify-nfc
Verify NFC tag for club entry.

#### GET /access/logs
Get user access history.

## Admin Endpoints

### User Management

#### GET /admin/users
Get all users (Admin only).

#### PUT /admin/users/:id/status
Update user status (Admin only).

#### GET /admin/users/:id/transactions
Get user transaction history (Admin only).

### Event Management

#### POST /admin/events
Create new event (Admin only).

#### PUT /admin/events/:id
Update event details (Admin only).

#### DELETE /admin/events/:id
Delete event (Admin only).

#### GET /admin/events/:id/registrations
Get event registrations (Admin only).

## Error Handling

### Standard Error Response
```json
{
  "success": false,
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Invalid input data",
    "details": [
      {
        "field": "email",
        "message": "Invalid email format"
      }
    ]
  }
}
```

### HTTP Status Codes
- `200`: Success
- `201`: Created
- `400`: Bad Request
- `401`: Unauthorized
- `403`: Forbidden
- `404`: Not Found
- `422`: Validation Error
- `500`: Internal Server Error

## Rate Limiting

- **General API**: 100 requests per minute per IP
- **Authentication**: 5 login attempts per minute per IP
- **Payment endpoints**: 10 requests per minute per user

## Webhooks

### M-PESA Callback
**Endpoint**: `POST /webhooks/mpesa/callback`

### Stripe Webhook
**Endpoint**: `POST /webhooks/stripe`

## API Versioning

- Current version: v1
- Version specified in URL: `/api/v1/`
- Backward compatibility maintained for 12 months
- Deprecation notices provided 6 months in advance