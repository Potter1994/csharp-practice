using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/api/todos")] // 用 /api/todos 避開 Minimal API 的 /todos
public class TodoController(ITodoService service) : ControllerBase
{
    [HttpGet] // GET /api/todos
    public async Task<IActionResult> GetAll()
    {
        var todos = await service.GetAllAsync();
        return Ok(todos.Select(t => t.ToResponse())); // 不是 Results.Ok()
    }

    [HttpGet("{id}")] // Get /api/todos/{id}
    public async Task<IActionResult> GetById(Guid id)
    {
        var todo = await service.GetByIdAsync(id);
        return todo is null ? Problem(statusCode: StatusCodes.Status404NotFound) : Ok(todo.ToResponse());
    }

    [HttpPost]
    public async Task<IActionResult> Add(CreateTodoRequest request)
    {
        var result = await service.CreateAsync(request);
        return result.Status switch
        {
            ResultStatus.Success => CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value!.ToResponse()),
            ResultStatus.Invalid => Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest),
            _ => Problem(),
        };
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateTodoRequest request)
    {
        var result = await service.UpdateAsync(id, request);
        return result.Status switch
        {
            ResultStatus.Success => NoContent(),
            ResultStatus.Invalid => Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest),
            ResultStatus.NotFound => Problem(statusCode: StatusCodes.Status404NotFound),
            _ => Problem(),
        };
    }
    [HttpPatch("{id}/done")]
    public async Task<IActionResult> Patch(Guid id, SetDoneRequest request)
    {
        var result = await service.PatchAsync(id, request);
        return result.Status switch
        {
            ResultStatus.Success => NoContent(),
            ResultStatus.Invalid => Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest),
            ResultStatus.NotFound => Problem(statusCode: StatusCodes.Status404NotFound),
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
            ResultStatus.Invalid => Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest),
            ResultStatus.NotFound => Problem(statusCode: StatusCodes.Status404NotFound),
            _ => Problem(),
        };
    }
}