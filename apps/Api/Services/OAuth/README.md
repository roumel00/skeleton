# OAuth Provider Architecture

This directory contains the extensible OAuth provider architecture that allows easy integration of new OAuth providers.

## How to Add a New OAuth Provider

### 1. Create a Provider Implementation

Create a new file `{Provider}OAuthProvider.cs` that implements `IOAuthProvider`:

```csharp
using Api.Services.OAuth;
using Api.Models;

public class FacebookOAuthProvider : IOAuthProvider
{
    public string ProviderName => "Facebook";
    
    // Implement required methods...
}
```

### 2. Register the Provider

In `Program.cs`, add:

```csharp
builder.Services.AddScoped<FacebookOAuthProvider>();
```

### 3. Update AuthenticationService

In `AuthenticationService.cs`, update the `GetOAuthProvider` method:

```csharp
private IOAuthProvider GetOAuthProvider(string provider)
{
    return provider.ToLower() switch
    {
        "google" => _serviceProvider.GetRequiredService<GoogleOAuthProvider>(),
        "facebook" => _serviceProvider.GetRequiredService<FacebookOAuthProvider>(),
        _ => throw new ArgumentException($"Unsupported OAuth provider: {provider}")
    };
}
```

### 4. Update User Entity

If the provider needs specific fields, add them to the `User` entity:

```csharp
public string? FacebookId { get; set; }
```

### 5. Update Database

Create and run a migration for any new fields.

### 6. Add Controller Endpoints

Add OAuth endpoints for the new provider:

```csharp
[HttpGet("facebook/start")]
public ActionResult FacebookStart()
{
    var authUrl = _authenticationService.GetOAuthAuthorizationUrl("facebook");
    return Redirect(authUrl);
}

[HttpGet("facebook/callback")]
public async Task<ActionResult> FacebookCallback([FromQuery] string? code, [FromQuery] string? error)
{
    // Similar to Google callback implementation
}
```

## Current Architecture

### Core Services

- **`ITokenService`**: Handles JWT generation and cookie management
- **`IAuthenticationService`**: Unified service for all authentication flows
- **`IOAuthProvider`**: Interface for OAuth providers

### OAuth Providers

- **`GoogleOAuthProvider`**: Google OAuth implementation

## Cookie Security Fix

The new `TokenService` fixes the production authentication issue by:

1. Always setting `Secure = true` for HTTPS requests
2. Using `SameSite = None` for cross-origin HTTPS requests
3. Supporting configurable cookie domains via `Frontend:CookieDomain` setting

## Configuration

Add OAuth provider configuration to `appsettings.json`:

```json
{
  "GoogleOAuth": {
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "RedirectUri": "https://your-api.com/api/oauth/google/callback"
  },
  "Frontend": {
    "CookieDomain": null // or ".yourdomain.com" for cross-subdomain
  }
}
```
