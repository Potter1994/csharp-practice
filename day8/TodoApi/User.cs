using System.ComponentModel.DataAnnotations;

public class User
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required string Email { get; set; }
    public required string Password { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public record RegisterRequestDto(
    [Required] [EmailAddress] string Email,
    [Required] [StringLength(100, MinimumLength = 8)] string Password
);
