using System.ComponentModel.DataAnnotations;

namespace Problem.Requests;

public record TaskRequest
{
    [Required] [MaxLength(100)] public string Name { get; set; } = null!;

    public bool IsCompleted { get; set; }
}