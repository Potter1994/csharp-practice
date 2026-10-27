public record UserResponse(Guid Id, string Email, DateTime CreatedAt);

public static class UserMappingExtensions
{
    public static UserResponse ToResponse(this User user) =>
        new(user.Id, user.Email, user.CreatedAt);
}
