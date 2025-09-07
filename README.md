# Full-Stack Skeleton Project

🚀 **Production-Ready Template**

A complete template with .NET 9 API backend and Next.js frontend, featuring built-in authentication and Google OAuth. **Skip the boilerplate and start building features immediately.** Checkout what it looks like [here](https://skeleton-orcin.vercel.app/)

## 🔐 Authentication System
- **JWT Authentication** with HTTP-only cookies
- **User registration & login**
- **Password reset via email**
- **Google OAuth integration**
- **Session management**

## 🎨 Modern Frontend
- **Next.js 15 with App Router**
- **shadcn/ui components**
- **Responsive design**
- **Protected routes**

## ⚡ Robust Backend
- **.NET 9 Web API**
- **Entity Framework Core**
- **PostgreSQL database**
- **Docker containerization**
- **Email service (Resend)**

## 🚀 Quick Start

### 1. Clone & Setup
```bash
# Clone the repository
git clone https://github.com/roumel00/skeleton.git
cd skeleton-project

# Setup environment
cp .env.example .env
# Run the fly secrets set for all the variables

# Update the database schema
ConnectionStrings__DefaultConnection='Host=db;Port=5432;Database=your-db-name;Username=your-db-user;Password=your-db-password' \
dotnet ef migrations add ReplaceIsGoogleUserWithOAuthProvider --context ApplicationDbContext
```

### 2. Start Services
```bash
# Start everything with Docker
docker compose up --build

# Access your apps
# Frontend: localhost:3000
# API: localhost:3030
```

### 3. Development Mode (Optional)
For frontend development with hot reloading:
```bash
# Start only API and database
docker compose up db api

# In another terminal, run the frontend in dev mode
cd apps/web
npm run dev
```

## 📁 Project Structure
```
skeleton-project/
├── apps/
│   ├── Api/                    # .NET 9 Web API
│   │   ├── Controllers/        # API endpoints
│   │   ├── Data/              # Database context
│   │   ├── Entities/          # Database entities
│   │   ├── Migrations/        # Database migrations
│   │   ├── Models/            # DTOs and request/response models
│   │   ├── Services/          # Business logic
│   │   └── Program.cs         # Application configuration
│   └── web/                   # Next.js Frontend
│       ├── app/               # App Router pages
│       ├── components/        # UI components
│       └── contexts/          # React contexts
├── docker-compose.yml         # Multi-container setup
└── .env                      # Environment variables
```

## 🚀 Ready to Build Something Amazing?

This template includes everything you need to start your next project. Authentication, database, modern UI components, and deployment-ready configuration.

**[Create Account]** | **[Sign In]**

## 🔧 Required Service Setup

### Google OAuth Setup (Optional)

To enable Google social login, you'll need to set up OAuth credentials:

1. **Go to Google Cloud Console**
   - Visit [Google Cloud Console](https://console.cloud.google.com/)
   - Create a new project or select an existing one

2. **Enable Google+ API**
   - Navigate to "APIs & Services" > "Library"
   - Search for "Google+ API" and enable it

3. **Create OAuth Credentials**
   - Go to "APIs & Services" > "Credentials"
   - Click "Create Credentials" > "OAuth 2.0 Client IDs"
   - Choose "Web application"
   - Add authorized redirect URIs:
     - Development: `http://localhost:3030/api/oauth/google/callback`
     - Production: `https://your-api-domain.com/api/oauth/google/callback`

4. **Update Environment Variables**
   ```bash
   GOOGLE_CLIENT_ID=your_client_id_here.apps.googleusercontent.com
   GOOGLE_CLIENT_SECRET=your_client_secret_here
   GOOGLE_OAUTH_REDIRECT_URI=http://localhost:3030/api/oauth/google/callback
   ```

### Email Service Setup (Resend)

For password reset functionality:

1. **Sign up for Resend**
   - Create account at [resend.com](https://resend.com)
   - Verify your domain or use their test domain

2. **Get API Key**
   - Go to [API Keys](https://resend.com/api-keys)
   - Create a new API key

3. **Update Environment Variables**
   ```bash
   RESEND_API_KEY=re_your_api_key_here
   APP_FROM_EMAIL=noreply@yourdomain.com
   ```

## ✨ Features Overview

### 🔐 Authentication System
- **JWT Authentication** with secure HTTP-only cookies
- **User Registration/Login** with email verification
- **Password Reset** via email with secure tokens
- **Google OAuth** social login integration
- **Session Management** with automatic token refresh

### 🎨 Frontend Features  
- **Modern UI** with shadcn/ui components
- **Responsive Design** that works on all devices
- **Protected Routes** with authentication guards
- **Form Validation** using react-hook-form + Zod
- **Toast Notifications** for user feedback
- **Loading States** and error handling

### ⚡ Backend Features
- **RESTful API** with clear endpoint structure
- **Database Migrations** with Entity Framework Core
- **Input Validation** and error handling
- **CORS Configuration** for cross-origin requests
- **Health Checks** for monitoring
- **Structured Logging** for debugging

## 🚀 Deployment

This template is designed for easy deployment with recommended hosting platforms that provide excellent developer experience and scalability.

### Development Environment

For local development with hot reload:

```bash
# Start all services (recommended)
docker compose up --build

# Or run services individually:
# Backend (.NET API)
cd apps/Api && dotnet run

# Frontend (Next.js)
cd apps/web && npm run dev

# Database (PostgreSQL)
docker run --name postgres -e POSTGRES_PASSWORD=password -p 5432:5432 -d postgres:16
```

### Production Deployment

#### Backend API + Database (Fly.io)

Deploy your .NET API and PostgreSQL database to Fly.io:

**1. Install Fly CLI and Login**
```bash
# Install Fly CLI
curl -L https://fly.io/install.sh | sh

# Login to your account
fly auth login
```

**2. Initialize Fly App**
```bash
cd apps/Api
fly launch --no-deploy  # Follow prompts to create app
```

**3. Set Production Environment Variables**
```bash
# Database (Fly.io will create managed PostgreSQL)
fly postgres create --name your-app-db

# Get connection string and set it
fly secrets set ConnectionStrings__DefaultConnection="your_postgres_connection_string"

# JWT Configuration
fly secrets set JwtSettings__SecretKey="$(openssl rand -base64 64)"
fly secrets set JwtSettings__Issuer="YourAppName"
fly secrets set JwtSettings__Audience="YourAppUsers"

# Email Service
fly secrets set Resend__ApiKey="your_resend_api_key"
fly secrets set App__FromEmail="noreply@yourdomain.com"

# Frontend URLs (will update after Vercel deployment)
fly secrets set Frontend__BaseUrl="https://your-vercel-app.vercel.app"
fly secrets set Frontend__Domain="your-vercel-app.vercel.app"

# Google OAuth (optional)
fly secrets set GoogleOAuth__ClientId="your_google_client_id"
fly secrets set GoogleOAuth__ClientSecret="your_google_client_secret"
fly secrets set GoogleOAuth__RedirectUri="https://your-fly-app.fly.dev/api/oauth/google/callback"
```

**4. Deploy**
```bash
fly deploy
```

#### Frontend (Vercel)

Deploy your Next.js frontend to Vercel:

**1. Connect to Vercel**
- Visit [vercel.com](https://vercel.com) and sign in
- Import your GitHub repository
- Set the root directory to `apps/web`

**2. Set Environment Variables in Vercel**
```bash
# API URL (use your Fly.io app URL)
NEXT_PUBLIC_API_BASE_URL=https://your-fly-app.fly.dev
```

**3. Deploy**
- Push to your main branch to trigger automatic deployment
- Or use Vercel CLI: `npx vercel --prod`

#### Alternative Deployment Options

**Backend Alternatives:**
- **Railway**: Similar to Fly.io with excellent developer experience
- **Azure App Service**: Microsoft's platform with .NET optimization
- **AWS ECS**: Container-based deployment with auto-scaling
- **DigitalOcean App Platform**: Simple container deployment

**Frontend Alternatives:**
- **Netlify**: Great for static sites with serverless functions
- **Cloudflare Pages**: Fast global CDN with edge computing
- **AWS Amplify**: Full-stack deployment with AWS integration

### Environment Variables Reference

#### Production Backend Variables (Fly.io/Railway/etc.)
```bash
ConnectionStrings__DefaultConnection=your_postgres_connection_string
JwtSettings__SecretKey=your_jwt_secret_64_chars_minimum
JwtSettings__Issuer=YourAppName
JwtSettings__Audience=YourAppUsers
Resend__ApiKey=re_your_resend_api_key
App__FromEmail=noreply@yourdomain.com
Frontend__BaseUrl=https://your-frontend-domain.com
Frontend__Domain=your-frontend-domain.com
GoogleOAuth__ClientId=your_google_client_id.apps.googleusercontent.com
GoogleOAuth__ClientSecret=your_google_client_secret
GoogleOAuth__RedirectUri=https://your-api-domain.com/api/oauth/google/callback
```

#### Production Frontend Variables (Vercel/Netlify/etc.)
```bash
NEXT_PUBLIC_API_BASE_URL=https://your-api-domain.com
```

## 🛠️ Customization Guide

### Adding New Features

1. **Backend**: Add controllers in `apps/Api/Controllers/`
2. **Database**: Create migrations with `dotnet ef migrations add YourMigration`
3. **Frontend**: Add pages in `apps/web/app/` using App Router
4. **Components**: Create reusable UI in `apps/web/components/`

### Styling and Branding

- **Colors**: Update `apps/web/app/globals.css` for color scheme
- **Components**: Customize `apps/web/components/ui/` for your design system
- **Layout**: Modify `apps/web/app/layout.js` for global layout changes

### Database Schema

Add new entities in `apps/Api/Entities/` and update `ApplicationDbContext.cs`:

```csharp
// Add to ApplicationDbContext.cs
public DbSet<YourEntity> YourEntities { get; set; }

// Create migration
dotnet ef migrations add AddYourEntity

// Apply to database
dotnet ef database update
```

## 🎯 Next Steps

1. **📧 Configure Email**: Set up Resend for password reset emails
2. **🔐 Setup OAuth**: Configure Google OAuth for social login (optional)  
3. **🎨 Customize Design**: Update colors, fonts, and components to match your brand
4. **📊 Add Analytics**: Integrate Google Analytics, Mixpanel, or similar
5. **🚀 Deploy**: Set up production deployment with Fly.io + Vercel
6. **📈 Monitor**: Add error tracking with Sentry or similar service
7. **🧪 Testing**: Add unit tests for your business logic
8. **📚 Documentation**: Update this README with your specific implementation details

## 🤝 Contributing

This is a template project - fork it and make it your own! If you create improvements that could benefit others, consider:

- Creating issues for bugs or feature requests
- Submitting pull requests for general improvements
- Sharing your customizations and extensions

---

**🚀 Happy building! You now have everything you need to create amazing full-stack applications.**
