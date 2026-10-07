public interface ITodoService
{
    IReadOnlyList<Todo> GetAll();
    Todo? GetById(Guid id);
    Todo Create(CreateTodoRequest request);
    bool Update(Guid id, CreateTodoRequest request);
    bool Delete(Guid id);
}