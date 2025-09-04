# Google OAuth Redirect Flow Setup

This document explains the new Google OAuth redirect flow implementation that replaces the previous popup-based authentication.

## Overview

The OAuth flow now works as follows:

1. User clicks "Continue with Google" button
2. User is redirected to Google's OAuth consent screen
3. After authentication, Google redirects back to your backend callback endpoint
4. Backend processes the authorization code and creates/updates user account
5. User is redirected to the dashboard with a valid session

## Configuration Required

### 1. Google Cloud Console Setup

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Navigate to APIs & Services > Credentials
3. Edit your OAuth 2.0 Client ID
4. Add the following redirect URI:
   - **Development**: `http://localhost:3030/api/auth/google/callback`
   - **Production**: `https://your-domain.com/api/auth/google/callback`

### 2. Backend Configuration

Update your `appsettings.json` and `appsettings.Development.json`:

```json
{
  "GoogleOAuth": {
    "ClientId": "your-google-client-id",
    "ClientSecret": "your-google-client-secret",
    "RedirectUri": "http://localhost:3030/api/auth/google/callback"
  }
}
```

For production, update the `RedirectUri` to match your production domain.

### 3. Environment Variables

Make sure your environment variables are set:

- `GoogleOAuth__ClientId`
- `GoogleOAuth__ClientSecret`
- `GoogleOAuth__RedirectUri`

## API Endpoints

### New Endpoints Added

- `GET /api/auth/google/redirect` - Initiates OAuth flow by redirecting to Google
- `GET /api/auth/google/callback` - Handles OAuth callback from Google

### Existing Endpoints

- `POST /api/auth/google` - Still available for backward compatibility (ID token flow)

## Frontend Changes

### GoogleOAuthButton Component

The component now:
- Uses a simple button instead of Google's Identity Services
- Redirects to the backend OAuth redirect endpoint
- No longer requires Google's JavaScript SDK

### SessionProvider

- Removed `signInWithGoogle` method (no longer needed)
- OAuth callback handling is automatic via page redirects

## Testing the Flow

1. Start your backend server
2. Start your frontend application
3. Navigate to the login page
4. Click "Continue with Google"
5. You should be redirected to Google's consent screen
6. After authentication, you should be redirected back to your dashboard

## Error Handling

- OAuth errors are handled by redirecting to `/login?error=error_message`
- The login page displays OAuth errors from URL parameters
- Backend logs detailed error information for debugging

## Security Considerations

- State parameter is generated for CSRF protection (can be enhanced with storage)
- Authorization codes are exchanged server-side only
- Client secret is never exposed to the frontend
- JWT tokens are set as HTTP-only cookies

## Migration Notes

- The old popup-based flow is completely replaced
- No changes needed to existing user accounts
- The `POST /api/auth/google` endpoint remains for any legacy integrations
