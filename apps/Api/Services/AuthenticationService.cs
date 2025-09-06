using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Api.Entities;
using Api.Models;
using Api.Data;
using Api.Services.OAuth;

namespace Api.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IServiceProvider _serviceProvider;

    public AuthenticationService(
        UserManager<User> userManager,
        ITokenService tokenService,
        ApplicationDbContext context,
        IEmailService emailService,
        IServiceProvider serviceProvider)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _context = context;
        _emailService = emailService;
        _serviceProvider = serviceProvider;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User with this email already exists"
                };
            }

            // Create new user
            var user = new User
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                EmailConfirmed = true, // For demo purposes
                IsGoogleUser = false
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"Registration failed: {errors}"
                };
            }

            // Generate JWT token and set cookie
            var token = _tokenService.GenerateJwtToken(user);
            _tokenService.SetAuthCookie(token);

            return new AuthResponseDto
            {
                Success = true,
                Message = "User registered successfully",
                Token = token,
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
                Message = $"An error occurred: {ex.Message}"
            };
        }
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }

            // Check if this is a Google-only user trying to log in with password
            if (user.IsGoogleUser && string.IsNullOrEmpty(user.PasswordHash))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "This account requires a different sign-in method."
                };
            }

            var isValidPassword = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isValidPassword)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }

            // Generate JWT token and set cookie
            var token = _tokenService.GenerateJwtToken(user);
            _tokenService.SetAuthCookie(token);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Login successful",
                Token = token,
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
                Message = $"An error occurred: {ex.Message}"
            };
        }
    }

    public async Task<AuthResponseDto> GetCurrentUserAsync()
    {
        try
        {
            var httpContextAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();
            var context = httpContextAccessor.HttpContext;
            
            if (context == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "No HTTP context available"
                };
            }

            // Add debugging information
            var authCookie = context.Request.Cookies["AuthToken"];

            // Check if user has a valid JWT token
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                // No valid authentication - this is normal for unauthenticated users
                return new AuthResponseDto
                {
                    Success = true,
                    User = null, // No user when not authenticated
                    Message = "No active session"
                };
            }

            var userIdClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                // This shouldn't happen if IsAuthenticated is true, but handle it gracefully
                return new AuthResponseDto
                {
                    Success = true,
                    User = null,
                    Message = "Invalid token format"
                };
            }

            var user = await _userManager.FindByIdAsync(userIdClaim.Value);
            if (user == null)
            {
                // User ID in token doesn't exist in database
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            return new AuthResponseDto
            {
                Success = true,
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
                Message = $"An error occurred: {ex.Message}"
            };
        }
    }

    public async Task<AuthResponseDto> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            
            // Always return success to prevent email enumeration attacks
            if (user != null)
            {
                // Check if this is a Google-only user
                if (user.IsGoogleUser && string.IsNullOrEmpty(user.PasswordHash))
                {
                    // Don't send password reset for Google-only users, but still return success
                    return new AuthResponseDto
                    {
                        Success = true,
                        Message = "If an account with that email exists, a password reset link has been sent."
                    };
                }

                // Clean up any existing unused tokens for this user
                var existingTokens = await _context.PasswordResetTokens
                    .Where(t => t.UserId == user.Id && !t.IsUsed)
                    .ToListAsync();
                
                _context.PasswordResetTokens.RemoveRange(existingTokens);

                // Generate secure random token
                var token = GenerateSecureToken();
                
                // Create password reset token entry
                var resetToken = new PasswordResetToken
                {
                    Token = token,
                    UserId = user.Id,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(30), // 30 minutes expiration
                    IsUsed = false
                };

                _context.PasswordResetTokens.Add(resetToken);
                await _context.SaveChangesAsync();

                // Send email
                var userName = !string.IsNullOrEmpty(user.FirstName) ? user.FirstName : user.Email!;
                await _emailService.SendPasswordResetEmailAsync(user.Email!, token, userName);
            }

            return new AuthResponseDto
            {
                Success = true,
                Message = "If an account with that email exists, a password reset link has been sent."
            };
        }
        catch (Exception ex)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = $"An error occurred: {ex.Message}"
            };
        }
    }

    public async Task<AuthResponseDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        try
        {
            // Find the token
            var resetToken = await _context.PasswordResetTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == resetPasswordDto.Token && !t.IsUsed);

            if (resetToken == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid or expired reset token."
                };
            }

            // Check if token is expired
            if (resetToken.ExpiresAt < DateTime.UtcNow)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Reset token has expired. Please request a new password reset."
                };
            }

            var user = resetToken.User;
            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            // Check if this is a Google-only user
            if (user.IsGoogleUser && string.IsNullOrEmpty(user.PasswordHash))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "This account uses Google sign-in and cannot have a password reset."
                };
            }

            // Remove current password
            var removePasswordResult = await _userManager.RemovePasswordAsync(user);
            if (!removePasswordResult.Succeeded)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Failed to reset password. Please try again."
                };
            }

            // Add new password
            var addPasswordResult = await _userManager.AddPasswordAsync(user, resetPasswordDto.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                var errors = string.Join(", ", addPasswordResult.Errors.Select(e => e.Description));
                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"Failed to set new password: {errors}"
                };
            }

            // Mark token as used
            resetToken.IsUsed = true;
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                Success = true,
                Message = "Password has been reset successfully. You can now log in with your new password."
            };
        }
        catch (Exception ex)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = $"An error occurred: {ex.Message}"
            };
        }
    }

    public string GetOAuthAuthorizationUrl(string provider)
    {
        var oauthProvider = GetOAuthProvider(provider);
        return oauthProvider.GetAuthorizationUrl();
    }

    public async Task<AuthResponseDto> HandleOAuthCallbackAsync(string provider, string code)
    {
        try
        {
            var oauthProvider = GetOAuthProvider(provider);
            var userInfo = await oauthProvider.GetUserInfoAsync(code);

            // Find or create user
            var user = await FindOrCreateOAuthUserAsync(userInfo);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Failed to create or find user"
                };
            }

            // Generate JWT token and set cookie
            var token = _tokenService.GenerateJwtToken(user);
            _tokenService.SetAuthCookie(token);

            return new AuthResponseDto
            {
                Success = true,
                Message = $"{provider} OAuth login successful",
                Token = token,
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

    public void SignOut()
    {
        _tokenService.ClearAuthCookie();
    }

    private IOAuthProvider GetOAuthProvider(string provider)
    {
        return provider.ToLower() switch
        {
            "google" => _serviceProvider.GetRequiredService<GoogleOAuthProvider>(),
            _ => throw new ArgumentException($"Unsupported OAuth provider: {provider}")
        };
    }

    private async Task<User?> FindOrCreateOAuthUserAsync(OAuthUserInfo userInfo)
    {
        // First, try to find by provider-specific ID (for Google, this is GoogleId)
        var existingUser = userInfo.Provider.ToLower() switch
        {
            "google" => await _context.Users.FirstOrDefaultAsync(u => u.GoogleId == userInfo.Id),
            _ => null
        };

        if (existingUser != null)
        {
            // Update user info in case it changed
            existingUser.FirstName = userInfo.FirstName ?? existingUser.FirstName;
            existingUser.LastName = userInfo.LastName ?? existingUser.LastName;
            existingUser.ProfilePictureUrl = userInfo.ProfilePictureUrl;
            existingUser.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return existingUser;
        }

        // Check if user exists with this email (might be a regular user wanting to link OAuth)
        var userByEmail = await _userManager.FindByEmailAsync(userInfo.Email);
        if (userByEmail != null)
        {
            // Link OAuth account to existing user
            switch (userInfo.Provider.ToLower())
            {
                case "google":
                    userByEmail.GoogleId = userInfo.Id;
                    break;
            }
            
            userByEmail.IsGoogleUser = userInfo.Provider.ToLower() == "google";
            userByEmail.ProfilePictureUrl = userInfo.ProfilePictureUrl;
            userByEmail.UpdatedAt = DateTime.UtcNow;
            
            await _userManager.UpdateAsync(userByEmail);
            return userByEmail;
        }

        // Create new user
        var newUser = new User
        {
            UserName = userInfo.Email,
            Email = userInfo.Email,
            EmailConfirmed = true, // OAuth emails are pre-verified
            FirstName = userInfo.FirstName,
            LastName = userInfo.LastName,
            ProfilePictureUrl = userInfo.ProfilePictureUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Set provider-specific fields
        switch (userInfo.Provider.ToLower())
        {
            case "google":
                newUser.GoogleId = userInfo.Id;
                newUser.IsGoogleUser = true;
                break;
        }

        var result = await _userManager.CreateAsync(newUser);
        return result.Succeeded ? newUser : null;
    }

    private string GenerateSecureToken()
    {
        // Generate a 256-bit (32-byte) random token
        using var rng = RandomNumberGenerator.Create();
        var tokenBytes = new byte[32];
        rng.GetBytes(tokenBytes);
        
        // Convert to URL-safe base64 string
        return Convert.ToBase64String(tokenBytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .Replace("=", "");
    }
}
