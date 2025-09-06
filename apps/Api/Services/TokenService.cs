using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Entities;

namespace Api.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"] ?? "YourSuperSecretKeyHere123!@#");
        var issuer = jwtSettings["Issuer"] ?? "YourApp";
        var audience = jwtSettings["Audience"] ?? "YourAppUsers";
        var expirationHours = int.Parse(jwtSettings["ExpirationHours"] ?? "24");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName),
            new Claim("IsGoogleUser", user.IsGoogleUser.ToString())
        };

        // Add profile picture URL if available
        if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
        {
            claims.Add(new Claim("ProfilePictureUrl", user.ProfilePictureUrl));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(expirationHours),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public void SetAuthCookie(string token)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return;

        var isHttps = context.Request.IsHttps;
        var host = context.Request.Host.Host.ToLower();
        
        // Check if we're in production environment
        var isProduction = !host.Contains("localhost") && !host.StartsWith("127.0.0.1");
        
        // For production HTTPS, always use secure cookies with SameSite=None
        var useSecureCookies = isProduction && isHttps;

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = useSecureCookies,
            SameSite = useSecureCookies ? SameSiteMode.None : SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddHours(24),
            Path = "/"
        };

        context.Response.Cookies.Append("AuthToken", token, cookieOptions);
    }

    public void ClearAuthCookie()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return;

        var isHttps = context.Request.IsHttps;
        var host = context.Request.Host.Host.ToLower();
        
        // Check if we're in production environment
        var isProduction = !host.Contains("localhost") && !host.StartsWith("127.0.0.1");
        
        // For production HTTPS, always use secure cookies with SameSite=None
        var useSecureCookies = isProduction && isHttps;
        
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = useSecureCookies,
            SameSite = useSecureCookies ? SameSiteMode.None : SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(-1), // Set to past date to delete
            Path = "/"
        };

        context.Response.Cookies.Append("AuthToken", "", cookieOptions);
    }
}
