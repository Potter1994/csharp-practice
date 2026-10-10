public interface ITodoRepository
{
    IReadOnlyList<Todo> GetAll();
    public Todo? GetById(Guid id);
    public void Add(Todo todo);
    public bool Update(Todo todo);
    public bool Delete(Guid id);
}