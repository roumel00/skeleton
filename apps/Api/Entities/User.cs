using Microsoft.AspNetCore.Identity;

namespace Api.Entities;

public class User : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? OAuthProvider { get; set; }

    // For google users
    public string? GoogleId { get; set; }
    public string? ProfilePictureUrl { get; set; }
}
