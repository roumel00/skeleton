using System.ComponentModel.DataAnnotations;

namespace Api.Entities;

public class PasswordResetToken
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(256)]
    public string Token { get; set; } = string.Empty;
    
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    [Required]
    public DateTime ExpiresAt { get; set; }
    
    public bool IsUsed { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public User User { get; set; } = null!;
}
