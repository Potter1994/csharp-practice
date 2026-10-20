public interface ITodoService
{
    Task<IReadOnlyList<Todo>> GetAllAsync();
    Task<Todo?> GetByIdAsync(Guid id);
    Task<Result<Todo>> CreateAsync(CreateTodoRequest request);
    Task<Result<Todo>> UpdateAsync(Guid id, UpdateTodoRequest request);
    Task<Result<Todo>> PatchAsync(Guid id, SetDoneRequest request);
    Task<Result> DeleteAsync(Guid id);
}