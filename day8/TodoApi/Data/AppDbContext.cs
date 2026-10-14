using Microsoft.EntityFrameworkCore;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // 寫成直接在 class 使用的 C# 12 primary constructor 能夠看起來更簡潔
    // public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    // {

    // }

    // public DbSet<Todo> Todos { get; set; } // ⚠️ nullable enabled 下會警告「未初始化」
    public DbSet<Todo> Todos => Set<Todo>(); // ✅ 建議這樣寫

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Todo>().HasKey(todo => todo.Id);
    }
}

/*
public DbSet<Todo> Todos { get; set; }        // 自動屬性：有一個隱藏的 backing field
public DbSet<Todo> Todos => Set<Todo>();      // 計算屬性：沒有欄位，每次呼叫 Set<Todo>()
這就是 Day 1 學的 expression-bodied member,完整寫法是:
public DbSet<Todo> Todos { get { return Set<Todo>(); } }   // 只有 get，沒有 set

而 => Set<Todo>() 根本不需要這個特例 —— 計算屬性沒有欄位,沒有東西需要初始化,自然不可能有這個警告。
*/