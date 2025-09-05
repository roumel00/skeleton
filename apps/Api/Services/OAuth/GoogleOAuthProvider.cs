using System.Text.Json;
using System.Web;
using Api.Models;

namespace Api.Services.OAuth;

public class GoogleOAuthProvider : IOAuthProvider
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public string ProviderName => "Google";

    public GoogleOAuthProvider(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
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

    public async Task<OAuthUserInfo> GetUserInfoAsync(string code)
    {
        // Exchange code for tokens
        var tokenResponse = await ExchangeCodeForTokensAsync(code);
        if (tokenResponse == null)
        {
            throw new InvalidOperationException("Failed to exchange code for tokens");
        }

        // Get user info from Google
        var googleUserInfo = await GetGoogleUserInfoAsync(tokenResponse.AccessToken);
        if (googleUserInfo == null)
        {
            throw new InvalidOperationException("Failed to get user info from Google");
        }

        return new OAuthUserInfo
        {
            Id = googleUserInfo.Id,
            Email = googleUserInfo.Email,
            FirstName = googleUserInfo.GivenName ?? "",
            LastName = googleUserInfo.FamilyName ?? "",
            ProfilePictureUrl = googleUserInfo.Picture,
            Provider = ProviderName
        };
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

    private async Task<GoogleUserInfo?> GetGoogleUserInfoAsync(string accessToken)
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
