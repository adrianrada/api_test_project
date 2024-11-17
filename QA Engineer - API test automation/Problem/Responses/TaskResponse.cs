namespace Problem.Responses;

public record TaskResponse
{
    public Guid Id { get; init; }

    public string Name { get; init; } = null!;

    public bool IsCompleted { get; init; }
}