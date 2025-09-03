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
```

### 3. Run with Docker
```bash
docker compose up --build
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
- ✅ Protected API endpoints
- ✅ Modern React components
- ✅ Responsive UI with Tailwind CSS
- ✅ Docker containerization
- ✅ PostgreSQL database
- ✅ Ready-to-extend architecture

## Next Steps

1. Customize the authentication logic
2. Add your business entities and endpoints
3. Style your components
4. Deploy to your preferred platform

---

**Ready to build!** 🚀
