public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/auth");
        group.MapGet(
            "/users",
            async (IUserService service) =>
            {
                var result = await service.GetAllAsync();
                return result.Status switch
                {
                    ResultStatus.Success => Results.Ok(result.Value?.Select(u => u.ToResponse())),
                    _ => Results.Problem(),
                };
            }
        );
        group.MapGet(
            "/users/{email}",
            async (string email, IUserService service) =>
            {
                var result = await service.GetByEmailAsync(email);
                return result.Status switch
                {
                    ResultStatus.Success => Results.Ok(result.Value?.ToResponse()),
                    ResultStatus.NotFound => Results.Problem(
                        detail: result.Error,
                        statusCode: StatusCodes.Status404NotFound
                    ),
                    _ => Results.Problem(),
                };
            }
        );
        group.MapPost(
            "/users",
            async (RegisterRequestDto request, IUserService service) =>
            {
                var result = await service.Register(request);
                return result.Status switch
                {
                    ResultStatus.Success => Results.Created(
                        $"/auth/users/{result.Value?.Email}",
                        result.Value?.ToResponse()
                    ),
                    ResultStatus.Conflict => Results.Problem(
                        detail: result.Error,
                        statusCode: StatusCodes.Status409Conflict
                    ),
                    _ => Results.Problem(),
                };
            }
        );
    }
}
