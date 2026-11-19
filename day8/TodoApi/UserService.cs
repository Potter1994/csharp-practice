using Microsoft.AspNetCore.Identity;

public class UserService(
    IUserRepository repo,
    IPasswordHasher<User> hasher,
    ITokenService jwtTokenService
) : IUserService
{
    // 真的是 HashPassword 產生的字串才會讓他一路走到 PBKDF2
    // 如果哪天雜湊更新了, 這個值也必須更新
    private static readonly string FakePassword =
        "AQAAAAIAAYagAAAAENFf53ab5Y9BbJC3qzuNQ0rL5Ai1Bgti/KPz58AsmEFcZDkYi8EY5rQPx1480cnqCw==";
    private static readonly User FakeUser = new()
    {
        Email = "fakeMail@example.com",
        Password = "abc123",
    };

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

    public async Task<Result<AccessToken>> Login(LoginRequestDto request)
    {
        var user = await repo.GetByEmailAsync(request.Email.Trim().ToLowerInvariant());
        var hash = user?.Password ?? FakePassword;
        var result = hasher.VerifyHashedPassword(user ?? FakeUser, hash, request.Password);

        if (user is null || result == PasswordVerificationResult.Failed)
            return Result<AccessToken>.Unauthorized("Email 或密碼錯誤");

        return Result<AccessToken>.Ok(jwtTokenService.Create(user));
    }
}
