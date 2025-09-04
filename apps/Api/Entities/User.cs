using Microsoft.AspNetCore.Identity;

namespace Api.Entities;

public class User : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // OAuth-related properties
    public string? GoogleId { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public bool IsOAuthUser { get; set; } = false;
}
