using System.Text.Json;
using System.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Api.Entities;
using Api.Data;
using Api.Models;

namespace Api.Services;

public interface IGoogleOAuthService
{
    string GetAuthorizationUrl();
    Task<AuthResponseDto> HandleCallbackAsync(string code);
}

public class GoogleOAuthService : IGoogleOAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDbContext _context;

    public GoogleOAuthService(
        HttpClient httpClient, 
        IConfiguration configuration, 
        UserManager<User> userManager,
        ApplicationDbContext context)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _userManager = userManager;
        _context = context;
    }

    public string GetAuthorizationUrl()
    {
        var clientId = _configuration["GoogleOAuth:ClientId"] 
            ?? throw new InvalidOperationException("Google OAuth ClientId is not configured");
        var redirectUri = _configuration["GoogleOAuth:RedirectUri"] 
            ?? throw new InvalidOperationException("Google OAuth RedirectUri is not configured");
        
        var scope = "openid email profile";
        var state = Guid.NewGuid().ToString(); // In production, store this for validation
        
        var queryParams = HttpUtility.ParseQueryString(string.Empty);
        queryParams["client_id"] = clientId;
        queryParams["redirect_uri"] = redirectUri;
        queryParams["response_type"] = "code";
        queryParams["scope"] = scope;
        queryParams["state"] = state;
        queryParams["access_type"] = "offline";
        queryParams["prompt"] = "consent";

        return $"https://accounts.google.com/o/oauth2/auth?{queryParams}";
    }

    public async Task<AuthResponseDto> HandleCallbackAsync(string code)
    {
        try
        {
            // Exchange code for tokens
            var tokenResponse = await ExchangeCodeForTokensAsync(code);
            if (tokenResponse == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Failed to exchange code for tokens"
                };
            }

            // Get user info from Google
            var userInfo = await GetUserInfoAsync(tokenResponse.AccessToken);
            if (userInfo == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Failed to get user info from Google"
                };
            }

            // Find or create user
            var user = await FindOrCreateUserAsync(userInfo);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Failed to create or find user"
                };
            }

            // Generate JWT token
            var jwtToken = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Google OAuth login successful",
                Token = jwtToken,
                User = new UserInfoDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CreatedAt = user.CreatedAt
                }
            };
        }
        catch (Exception ex)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = $"OAuth error: {ex.Message}"
            };
        }
    }

    private async Task<GoogleTokenResponse?> ExchangeCodeForTokensAsync(string code)
    {
        var clientId = _configuration["GoogleOAuth:ClientId"]!;
        var clientSecret = _configuration["GoogleOAuth:ClientSecret"]!;
        var redirectUri = _configuration["GoogleOAuth:RedirectUri"]!;

        var parameters = new Dictionary<string, string>
        {
            ["client_id"] = clientId,
            ["client_secret"] = clientSecret,
            ["code"] = code,
            ["grant_type"] = "authorization_code",
            ["redirect_uri"] = redirectUri
        };

        var content = new FormUrlEncodedContent(parameters);
        var response = await _httpClient.PostAsync("https://oauth2.googleapis.com/token", content);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<GoogleTokenResponse>(json, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });
    }

    private async Task<GoogleUserInfo?> GetUserInfoAsync(string accessToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<GoogleUserInfo>(json, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });
    }

    private async Task<User?> FindOrCreateUserAsync(GoogleUserInfo userInfo)
    {
        // First, try to find by Google ID
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.GoogleId == userInfo.Id);

        if (existingUser != null)
        {
            // Update user info in case it changed
            existingUser.FirstName = userInfo.GivenName ?? existingUser.FirstName;
            existingUser.LastName = userInfo.FamilyName ?? existingUser.LastName;
            existingUser.ProfilePictureUrl = userInfo.Picture;
            existingUser.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return existingUser;
        }

        // Check if user exists with this email (might be a regular user wanting to link Google)
        var userByEmail = await _userManager.FindByEmailAsync(userInfo.Email);
        if (userByEmail != null)
        {
            // Link Google account to existing user
            userByEmail.GoogleId = userInfo.Id;
            userByEmail.IsGoogleUser = true;
            userByEmail.ProfilePictureUrl = userInfo.Picture;
            userByEmail.UpdatedAt = DateTime.UtcNow;
            
            await _userManager.UpdateAsync(userByEmail);
            return userByEmail;
        }

        // Create new user
        var newUser = new User
        {
            UserName = userInfo.Email,
            Email = userInfo.Email,
            EmailConfirmed = true, // Google emails are pre-verified
            FirstName = userInfo.GivenName ?? "",
            LastName = userInfo.FamilyName ?? "",
            GoogleId = userInfo.Id,
            IsGoogleUser = true,
            ProfilePictureUrl = userInfo.Picture,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(newUser);
        return result.Succeeded ? newUser : null;
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"] ?? "YourSuperSecretKeyHere123!@#");
        var issuer = jwtSettings["Issuer"] ?? "YourApp";
        var audience = jwtSettings["Audience"] ?? "YourAppUsers";
        var expirationHours = int.Parse(jwtSettings["ExpirationHours"] ?? "24");

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
                new Claim("IsGoogleUser", user.IsGoogleUser.ToString()),
                new Claim("ProfilePictureUrl", user.ProfilePictureUrl ?? "")
            }),
            Expires = DateTime.UtcNow.AddHours(expirationHours),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

// DTOs for Google API responses
public class GoogleTokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
}

public class GoogleUserInfo
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool VerifiedEmail { get; set; }
    public string Name { get; set; } = string.Empty;
    public string GivenName { get; set; } = string.Empty;
    public string FamilyName { get; set; } = string.Empty;
    public string Picture { get; set; } = string.Empty;
}