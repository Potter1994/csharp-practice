public interface ITodoRepository
{
    Task<IReadOnlyList<Todo>> GetAllAsync();
    public Task<Todo?> GetByIdAsync(Guid id);
    public Task AddAsync(Todo todo);
    public Task<bool> UpdateAsync(Todo todo);
    public Task<bool> DeleteAsync(Guid id);
}