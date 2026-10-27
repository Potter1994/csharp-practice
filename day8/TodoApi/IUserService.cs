public interface IUserService
{
    Task<Result<IReadOnlyList<User>>> GetAllAsync();
    Task<Result<User>> GetByEmailAsync(string email);
    Task<Result<User>> Register(RegisterRequestDto request);
}
