public class TodoService(ITodoRepository repo) : ITodoService
{

    public IReadOnlyList<Todo> GetAll()
    {
        return repo.GetAll();
    }

    public Todo? GetById(Guid id)
    {
        return repo.GetById(id);
    }
    public Todo Create(CreateTodoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            //  這邊應該要丟不知道什麼錯到 Endpoint 那邊讓他去處理錯誤訊息   
        }
        Todo newTodo = new(Guid.CreateVersion7(), request.Title, request.IsDone);
        repo.Add(newTodo);
        return newTodo;
    }
    public bool Update(Guid id, CreateTodoRequest request)
    {
        Todo UpdatedTodo = new(id, request.Title, request.IsDone);
        return repo.Update(UpdatedTodo);
    }
    public bool Delete(Guid id)
    {
        return repo.Delete(id);
    }
}