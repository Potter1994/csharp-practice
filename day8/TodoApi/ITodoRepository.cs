public interface ITodoRepository
{
    Task<IReadOnlyList<Todo>> GetAll();
    public Task<Todo?> GetById(Guid id);
    public Task Add(Todo todo);
    public Task<bool> Update(Todo todo);
    public Task<bool> Delete(Guid id);
}