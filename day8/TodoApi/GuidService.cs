public class GuidService : IGuidService
{
    public Guid Id { get; } = Guid.CreateVersion7();
}