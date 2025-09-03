# Full-Stack Skeleton Project

A production-ready full-stack application with .NET 9 API backend and Next.js frontend, featuring JWT authentication, Docker containerization, and modern development practices.

## 🏗️ Project Structure

```
skeleton-project/
├── apps/
│   ├── Api/              # .NET 9 Web API
│   │   ├── Controllers/  # API controllers
│   │   ├── Data/        # Entity Framework DbContext
│   │   ├── Entities/    # Database entities
│   │   ├── Models/      # DTOs and view models
│   │   └── Services/    # Business logic services
│   └── web/             # Next.js Frontend
│       ├── app/         # Next.js App Router pages
│       ├── components/  # React components
│       ├── contexts/    # React contexts
│       └── lib/         # Utility functions
├── docker-compose.yml   # Multi-service orchestration
└── README.md           # This file
```

## ✨ Features

### Backend (.NET 9 API)
- ✅ JWT-based authentication and authorization
- ✅ User registration and login with validation
- ✅ ASP.NET Core Identity for user management
- ✅ Entity Framework Core with PostgreSQL
- ✅ Password hashing and security
- ✅ Protected endpoints with `[Authorize]` attribute
- ✅ CORS configuration
- ✅ Comprehensive input validation
- ✅ Proper error handling and responses
- ✅ Docker containerization

### Frontend (Next.js)
- ✅ Modern Next.js 14+ with App Router
- ✅ React Server Components
- ✅ Authentication state management
- ✅ Protected routes and layouts
- ✅ Responsive UI with Tailwind CSS
- ✅ Component library (shadcn/ui)
- ✅ Client-side session management
- ✅ Form handling and validation
- ✅ Docker containerization

## 🚀 Quick Start

### Prerequisites
- Docker and Docker Compose
- .NET 9 SDK (for local development)
- Node.js 18+ (for local development)

### 1. Environment Setup

Create a `.env` file in the root directory:

```bash
# Database Configuration
DB_PASSWORD=your_secure_database_password_here

# JWT Configuration - Generate with: openssl rand -base64 64
JWT_SECRET=your_secure_jwt_secret_key_here

# CORS Configuration
CORS_ORIGIN=http://localhost:3000
```

### 2. Generate JWT Secret

```bash
openssl rand -base64 64
```

Copy the output and use it as your `JWT_SECRET` value in the `.env` file.

### 3. Run with Docker Compose

```bash
# Start all services
docker compose up --build

# Run in background
docker compose up -d --build
```

This will start:
- **PostgreSQL Database** on port 5432
- **.NET API** on port 3030
- **Next.js Frontend** on port 3000

### 4. Access the Application

- **Frontend**: http://localhost:3000
- **API**: http://localhost:3030
- **API Documentation**: http://localhost:3030/swagger (if enabled)

## 🔧 Development

### Local Development (without Docker)

#### Backend Setup
```bash
cd apps/Api
dotnet restore
dotnet run
```

#### Frontend Setup
```bash
cd apps/web
npm install
npm run dev
```

### Environment Variables

#### Backend (.NET API)
- `ConnectionStrings__DefaultConnection` - Database connection string
- `JwtSettings__SecretKey` - JWT signing secret
- `JwtSettings__Issuer` - JWT issuer
- `JwtSettings__Audience` - JWT audience
- `CORS__AllowedOrigins` - Allowed CORS origins

#### Frontend (Next.js)
- `NEXT_PUBLIC_API_BASE_URL` - API base URL (set in Docker Compose)

## 📚 API Documentation

### Authentication Endpoints

#### Register User
```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "password": "SecurePass123!",
  "confirmPassword": "SecurePass123!"
}
```

**Response:**
```json
{
  "success": true,
  "message": "User registered successfully",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "user-guid",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "createdAt": "2024-01-01T00:00:00Z"
  }
}
```

#### Login User
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePass123!"
}
```

**Response:** Same format as register response

### Test Endpoints

#### Public Endpoint
```http
GET /api/test/public
```
No authentication required

#### Protected Endpoint
```http
GET /api/test/protected
Authorization: Bearer {your-jwt-token}
```
Requires valid JWT token

## 🔐 Security Features

### Password Requirements
- Minimum 6 characters
- At least one uppercase letter
- At least one lowercase letter
- At least one number
- At least one special character

### Security Implementations
- **Password Hashing**: ASP.NET Core Identity's secure hashing
- **JWT Tokens**: Stateless authentication with configurable expiration
- **Input Validation**: Comprehensive validation using Data Annotations
- **CORS**: Configurable Cross-Origin Resource Sharing
- **HTTPS**: Enforced in production environments

## 🗄️ Database

### Technology Stack
- **Database**: PostgreSQL 16
- **ORM**: Entity Framework Core
- **Approach**: Code First migrations

### Tables Created
- `AspNetUsers` - User accounts
- `AspNetRoles` - User roles
- `AspNetUserRoles` - User-role relationships
- `AspNetUserClaims` - User claims
- `AspNetUserLogins` - External login providers
- `AspNetUserTokens` - User tokens

### Database Migrations
```bash
# Add migration
cd apps/Api
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update
```

## 🧪 Testing

### Testing the API with curl

1. **Register a new user:**
```bash
curl -X POST http://localhost:3030/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "firstName": "Test",
    "lastName": "User",
    "password": "TestPass123!",
    "confirmPassword": "TestPass123!"
  }'
```

2. **Login and get token:**
```bash
curl -X POST http://localhost:3030/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "TestPass123!"
  }'
```

3. **Access protected endpoint:**
```bash
curl -X GET http://localhost:3030/api/test/protected \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE"
```

## 🚢 Production Deployment

### Production Considerations

1. **Environment Variables**: Store all sensitive configuration in environment variables
2. **HTTPS**: Always use HTTPS in production
3. **Database**: Use a production-grade database (Azure SQL, AWS RDS, etc.)
4. **Email Confirmation**: Enable `RequireConfirmedEmail = true`
5. **Logging**: Implement comprehensive logging and monitoring
6. **Rate Limiting**: Add rate limiting for authentication endpoints
7. **Secrets Management**: Use proper secrets management (Azure Key Vault, AWS Secrets Manager)

### Docker Production Build

```bash
# Build production images
docker compose -f docker-compose.prod.yml up --build

# Or build individual services
docker build -t skeleton-api ./apps/Api
docker build -t skeleton-web ./apps/web
```

## 🛠️ Troubleshooting

### Common Issues

#### Database Connection
- Ensure PostgreSQL is running
- Verify connection string in environment variables
- Check that `DB_PASSWORD` is set in `.env` file

#### JWT Token Issues
- Verify `JWT_SECRET` is set and sufficiently long
- Check token is being sent in Authorization header
- Ensure token hasn't expired

#### CORS Issues
- Verify `CORS_ORIGIN` environment variable
- Check frontend URL matches CORS configuration

#### Docker Issues
```bash
# Rebuild containers
docker compose down
docker compose up --build

# View logs
docker compose logs api
docker compose logs web
docker compose logs db

# Remove volumes (caution: deletes data)
docker compose down -v
```

#### Frontend Build Issues
```bash
cd apps/web
rm -rf .next node_modules
npm install
npm run build
```

#### Backend Build Issues
```bash
cd apps/Api
dotnet clean
dotnet restore
dotnet build
```

## 🎯 Next Steps

Consider implementing:

### Backend Enhancements
- [ ] Role-based authorization
- [ ] Refresh tokens
- [ ] Password reset functionality
- [ ] Email confirmation
- [ ] Two-factor authentication
- [ ] Audit logging
- [ ] API versioning
- [ ] Rate limiting
- [ ] Health checks

### Frontend Enhancements
- [ ] User profile management
- [ ] Password reset flow
- [ ] Email verification
- [ ] Dark mode
- [ ] Internationalization (i18n)
- [ ] Progressive Web App (PWA)
- [ ] State management (Zustand/Redux)
- [ ] Form validation library (Zod)

### DevOps & Infrastructure
- [ ] CI/CD pipelines
- [ ] Unit and integration tests
- [ ] Load testing
- [ ] Monitoring and alerting
- [ ] Backup strategies
- [ ] CDN integration
- [ ] Multi-environment setup

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## 📞 Support

For support and questions:
- Create an issue in the repository
- Review the troubleshooting section
- Check the documentation

---

Built with ❤️ using .NET 9, Next.js, PostgreSQL, and Docker.
