using Api.Models;

namespace Api.Services.OAuth;

public interface IOAuthProvider
{
    string ProviderName { get; }
    string GetAuthorizationUrl();
    Task<OAuthUserInfo> GetUserInfoAsync(string code, string state);
}

public class OAuthUserInfo
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public string Provider { get; set; } = string.Empty;
}
