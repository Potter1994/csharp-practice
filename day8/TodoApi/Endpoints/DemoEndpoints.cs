static class DemoEndpoints
{
    internal static void MapDemoEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", () => "Hello World!");
        app.MapGet("/hello", async (HttpContext context, /*[FromQuery] 可以寫在前面更清楚*/ string name) =>
        {
            Console.WriteLine($"QueryString 也能直接寫在參數取得 name: {name}");
            foreach (var key in context.Request.Query.Keys)
            {
                await context.Response.WriteAsync($"{key}: {context.Request.Query[key]}\r\n");
            }
        });

        app.MapGet("/hello/{name}", (string? name) => $"Hello {name}");

        app.MapGet("/guid", (IGuidService service1, IGuidService service2) =>
        {
            return new
            {
                guid1 = service1.Id,
                guid2 = service2.Id,
                same = service1.Id == service2.Id,
            };
        });
    }
}