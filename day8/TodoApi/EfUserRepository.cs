using Microsoft.EntityFrameworkCore;
using Npgsql;

public class EfUserRepository(AppDbContext db) : IUserRepository
{
    public async Task<IReadOnlyList<User>> GetAllAsync() =>
        await db.Users.AsNoTracking().OrderBy(u => u.Id).ToListAsync();

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await db.Users.AsNoTracking().AnyAsync(u => u.Email == email);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await db.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> TryAddAsync(User user)
    {
        db.Users.Add(user);
        try
        {
            await db.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex)
            when (ex.InnerException
                    is PostgresException
                    {
                        SqlState: PostgresErrorCodes.UniqueViolation,
                        ConstraintName: "IX_Users_Email"
                    }
            )
        {
            db.Entry(user).State = EntityState.Detached;
            return false;
        }
    }
}
