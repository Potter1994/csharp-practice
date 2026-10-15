using Microsoft.EntityFrameworkCore;

public class EfTodoRepository(AppDbContext db) : ITodoRepository
{
    public async Task AddAsync(Todo todo)
    {
        db.Todos.Add(todo); // 將物件添加到 DbSet 中
        await db.SaveChangesAsync(); // 將變更儲存到資料庫
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var todo = await db.Todos.FirstOrDefaultAsync(todo => todo.Id == id);

        if (todo != null)
        {
            db.Remove(todo);
            await db.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<IReadOnlyList<Todo>> GetAllAsync()
    {
        // AsNoTracking() 只加在呼叫路徑明確唯讀的地方。
        return await db.Todos.AsNoTracking().OrderBy(t => t.Id).ToListAsync();

        /*
            AsNoTracking() 省掉的是三件事,而它們全部發生在 SQL 執行完之後:
            不為每個實體建立原始值快照(每個屬性都要複製一份)
            不把實體插入 change tracker 的 identity map
            不做身分解析 —— 追蹤模式下,同一個 Id 在一次請求裡查兩次會回傳同一個物件實例,這需要查表比對  
        */
    }

    public async Task<Todo?> GetByIdAsync(Guid id)
    {
        return await db.Todos.FindAsync(id);
    }

    public async Task<bool> UpdateAsync(Todo todo)
    {
        var result = await db.Todos.FirstOrDefaultAsync(t => t.Id == todo.Id);
        if (result != null)
        {
            result.Title = todo.Title;
            result.IsDone = todo.IsDone;
            await db.SaveChangesAsync();
            return true;
        }
        return false;
    }
}