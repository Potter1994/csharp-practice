public interface ITodoService
{
    Task<IReadOnlyList<Todo>> GetAllAsync();
    Task<Todo?> GetByIdAsync(Guid id);
    Task<Result<Todo>> CreateAsync(CreateTodoRequest request);
    Task<Result<Todo>> UpdateAsync(Guid id, CreateTodoRequest request);
    Task<Result> DeleteAsync(Guid id);
}