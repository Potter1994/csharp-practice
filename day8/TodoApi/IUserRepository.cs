public interface IUserRepository
{
    Task<IReadOnlyList<User>> GetAllAsync();
    Task<User?> GetByEmailAsync(string email);
    Task<bool> TryAddAsync(User user);
    Task<bool> ExistsByEmailAsync(string email);
}
