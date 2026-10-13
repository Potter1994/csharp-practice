using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/todos")] // 用 /api/todos 避開 Minimal API 的 /todos
public class TodoController(ITodoService service) : ControllerBase
{
    [HttpGet] // GET /api/todos
    public async Task<IActionResult> GetAll()
    {
        var todos = await service.GetAllAsync();
        return Ok(todos); // 不是 Results.Ok()
    }

    [HttpGet("{id}")] // Get /api/todos/{id}
    public async Task<IActionResult> GetById(Guid id)
    {
        var todo = await service.GetByIdAsync(id);
        return todo is null ? NotFound() : Ok(todo);
    }

    [HttpPost]
    public async Task<IActionResult> Add(CreateTodoRequest request)
    {
        var result = await service.CreateAsync(request);
        return result.Status switch
        {
            ResultStatus.Success => CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value),
            ResultStatus.Invalid => BadRequest(new { message = result.Error }),
            _ => Problem(),
        };
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, CreateTodoRequest request)
    {
        var result = await service.UpdateAsync(id, request);
        return result.Status switch
        {
            ResultStatus.Success => NoContent(),
            ResultStatus.Invalid => BadRequest(new { message = result.Error }),
            ResultStatus.NotFound => NotFound(new { message = result.Error }),
            _ => Problem(),
        };
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await service.DeleteAsync(id);
        return result.Status switch
        {
            ResultStatus.Success => NoContent(),
            ResultStatus.Invalid => BadRequest(new { message = result.Error }),
            ResultStatus.NotFound => NotFound(new { message = result.Error }),
            _ => Problem(),
        };
    }
}