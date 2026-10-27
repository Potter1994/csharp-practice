using Microsoft.AspNetCore.Identity;

public class UserService(IUserRepository repo, IPasswordHasher<User> hasher) : IUserService
{
    public async Task<Result<User>> Register(RegisterRequestDto request)
    {
        const string DuplicateEmail = "此 Email 已經註冊";

        string normalized = request.Email.Trim().ToLowerInvariant();

        // 第二層:一般情況的錯誤訊息,省掉一次失敗的 INSERT
        if (await repo.ExistsByEmailAsync(normalized))
            return Result<User>.Conflict(DuplicateEmail);

        User user = new User { Email = normalized, Password = string.Empty };
        user.Password = hasher.HashPassword(user, request.Password);

        // 第三層:守 TOCTOU 的競態窗口
        if (!await repo.TryAddAsync(user))
            return Result<User>.Conflict(DuplicateEmail);
        return Result<User>.Ok(user);
    }

    public async Task<Result<IReadOnlyList<User>>> GetAllAsync() =>
        Result<IReadOnlyList<User>>.Ok(await repo.GetAllAsync());

    public async Task<Result<User>> GetByEmailAsync(string email)
    {
        var result = await repo.GetByEmailAsync(email.Trim().ToLowerInvariant());
        if (result is null)
            return Result<User>.NotFound("此 Email 尚未註冊");
        return Result<User>.Ok(result);
    }
}
