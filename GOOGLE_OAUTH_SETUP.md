# Google OAuth Setup Instructions

This document provides step-by-step instructions for setting up Google OAuth authentication in your application.

## Prerequisites

1. Google Cloud Console account
2. A Google Cloud Project

## Step 1: Create Google OAuth Credentials

1. Go to the [Google Cloud Console](https://console.cloud.google.com/)
2. Select your project or create a new one
3. Navigate to **APIs & Services** > **Credentials**
4. Click **+ CREATE CREDENTIALS** > **OAuth client ID**
5. If prompted, configure the OAuth consent screen first:
   - Choose **External** for user type (unless you have a Google Workspace)
   - Fill in the required fields:
     - App name: Your app name
     - User support email: Your email
     - Developer contact information: Your email
   - Add scopes: `email`, `profile`, `openid`
   - Add test users if needed

6. Create OAuth client ID:
   - Application type: **Web application**
   - Name: Your app name
   - Authorized JavaScript origins:
     - `http://localhost:3000` (for development)
     - Your production domain (e.g., `https://yourdomain.com`)
   - Authorized redirect URIs:
     - `http://localhost:3000` (for development)
     - Your production domain (e.g., `https://yourdomain.com`)

7. Save the credentials - you'll get a **Client ID** and **Client Secret**

## Step 2: Configure Backend (API)

### Environment Configuration

Update your configuration files with the Google OAuth credentials:

**appsettings.Development.json:**
```json
{
  "GoogleOAuth": {
    "ClientId": "YOUR_GOOGLE_CLIENT_ID_HERE",
    "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET_HERE"
  }
}
```

**For production, set environment variables:**
- `GoogleOAuth__ClientId`
- `GoogleOAuth__ClientSecret`

## Step 3: Configure Frontend (Web App)

### Environment Variables

Create a `.env.local` file in the `/apps/web` directory:

```bash
# Google OAuth Configuration
NEXT_PUBLIC_GOOGLE_CLIENT_ID=YOUR_GOOGLE_CLIENT_ID_HERE

# API Configuration (if different from default)
NEXT_PUBLIC_API_BASE_URL=http://localhost:3030
```

**For production deployment, set these environment variables in your hosting platform.**

## Step 4: Database Migration

The application has been updated with new OAuth-related fields. Run the database migration:

```bash
cd apps/Api
dotnet ef database update
```

## Step 5: Test the Integration

1. Start your API server:
   ```bash
   cd apps/Api
   dotnet run
   ```

2. Start your web application:
   ```bash
   cd apps/web
   npm run dev
   ```

3. Navigate to the login page and test the "Continue with Google" button

## Features Added

- **Google OAuth Authentication**: Users can sign in/up with Google
- **Account Linking**: Existing email accounts are automatically linked with Google OAuth
- **Profile Pictures**: Google profile pictures are automatically imported
- **Seamless Experience**: OAuth users don't need to set passwords

## API Endpoints

- `POST /api/auth/google`: Authenticate with Google ID token

## Security Notes

- Google ID tokens are validated server-side using Google's verification library
- Existing password-based authentication remains fully functional
- OAuth users are clearly marked in the database
- Email verification is automatically handled for Google users

## Troubleshooting

1. **"Google authentication is not available"**: Check that your Google Client ID is correctly set in environment variables
2. **"Invalid Google token"**: Ensure your Client ID matches between frontend and backend
3. **CORS issues**: Make sure your domain is added to authorized origins in Google Console
4. **Redirect URI mismatch**: Verify the redirect URIs in Google Console match your application URLs

## Next Steps

- Consider adding other OAuth providers (Facebook, Microsoft, etc.)
- Implement profile picture display in the UI
- Add OAuth provider management in user settings
