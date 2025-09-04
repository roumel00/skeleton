# Full-Stack Skeleton Project

A copy-paste starting point for building full-stack applications with .NET 9 API backend and Next.js frontend.

## What's Included

- **Backend**: .NET 9 Web API with JWT authentication, Entity Framework Core, and PostgreSQL
- **Frontend**: Next.js 14+ with App Router, authentication, and modern UI components
- **Database**: PostgreSQL with Docker
- **Authentication**: JWT-based auth with user registration/login
- **Docker**: Complete containerization setup

## Quick Start

### 1. Clone and Setup
```bash
git clone <your-repo>
cd skeleton-project
```

### 2. Environment Variables
Create a `.env` file in the root:
```bash
# Database Configuration
DB_USER=jake
DB_PASSWORD=your_password_here
DB_NAME=skeleton

# JWT Configuration
JWT_SECRET=$(openssl rand -base64 64)
JWT_ISSUER=SkeletonApi
JWT_AUDIENCE=SkeletonFrontend

# CORS Configuration
CORS_ORIGIN=http://localhost:3000

# API Configuration
API_BASE_URL=http://localhost:3030

# Email Service (for password reset)
RESEND_API_KEY=your_resend_api_key_here
```

### 3. Run with Docker
```bash
docker compose down && docker compose up --build
```

### 4. Access Your Apps
- **Frontend**: http://localhost:3000
- **API**: http://localhost:3030

## Project Structure
```
skeleton-project/
├── apps/
│   ├── Api/          # .NET 9 Web API
│   └── web/          # Next.js Frontend
├── docker-compose.yml
└── README.md
```

## What You Get

- ✅ User registration and login
- ✅ JWT authentication
- ✅ Password reset functionality with email
- ✅ Protected API endpoints
- ✅ Modern React components
- ✅ Responsive UI with Tailwind CSS
- ✅ Docker containerization
- ✅ PostgreSQL database
- ✅ Ready-to-extend architecture

## Deployment

### Backend & Database on Fly.io

The backend API and PostgreSQL database are deployed to Fly.io. The API is configured to run on port 3030 and includes:

- JWT authentication with HTTP-only cookies
- CORS configuration for cross-origin requests
- PostgreSQL database with Entity Framework Core
- Automatic database creation on startup

### Frontend on Vercel

The Next.js frontend is deployed to Vercel and configured to communicate with the Fly.io API.

### Required Environment Variables

To deploy your own version, you must set the following environment variables in Fly.io:

```bash
# Set database connection string
fly secrets set ConnectionStrings__DefaultConnection="your_postgres_connection_string"

# Set JWT configuration
fly secrets set JwtSettings__SecretKey="your_jwt_secret_key"
fly secrets set JwtSettings__Issuer="your-app-name"
fly secrets set JwtSettings__Audience="your-app-users"

# Set CORS allowed origins (your Vercel app URL)
fly secrets set CORS__AllowedOrigins="https://your-vercel-app.vercel.app"

# Set Resend API key for password reset emails
fly secrets set Resend__ApiKey="your_resend_api_key"
```

### Frontend Environment Variables

For your Vercel deployment, set this environment variable:

```bash
NEXT_PUBLIC_API_BASE_URL=https://your-fly-app-name.fly.dev
```

## Email Service Setup

This project uses [Resend](https://resend.com) for sending password reset emails. To enable the password reset functionality:

1. **Sign up for Resend**: Create an account at [resend.com](https://resend.com)
2. **Get your API key**: Copy your API key from the Resend dashboard
3. **Set the environment variable**: Add `RESEND_API_KEY=your_api_key_here` to your `.env` file
4. **For production**: Set the `Resend__ApiKey` secret in Fly.io as shown in the deployment section

Without the Resend API key, the password reset functionality will not work and will throw an error when users try to reset their passwords.

## Next Steps

1. Get a Resend API key and configure it for password reset emails
2. Customize the authentication logic
3. Add your business entities and endpoints
4. Style your components
5. Set up your own Fly.io and Vercel deployments with the required environment variables

---

**Ready to build!** 🚀
