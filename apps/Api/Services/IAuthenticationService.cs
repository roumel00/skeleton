using Api.Models;
using Api.Services.OAuth;

namespace Api.Services;

public interface IAuthenticationService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<AuthResponseDto> GetCurrentUserAsync();
    Task<AuthResponseDto> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
    Task<AuthResponseDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    
    // OAuth methods
    string GetOAuthAuthorizationUrl(string provider);
    Task<AuthResponseDto> HandleOAuthCallbackAsync(string provider, string code, string state);
    
    // Session management
    void SignOut();
}
