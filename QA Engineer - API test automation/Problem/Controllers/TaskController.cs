using Microsoft.AspNetCore.Mvc;
using Problem.Requests;
using Problem.Responses;

namespace Problem.Controllers;

[ApiController]
[Route("tasks")]
public class TaskController : ControllerBase
{
    private static readonly List<TaskModel> Tasks =
    [
        new()
        {
            Id = Guid.Parse("9afe762c-96f9-42e7-90d8-2ac7cb88d570"),
            Name = "Old Task"
        }
    ];

    [HttpGet]
    public ActionResult<IEnumerable<TaskResponse>> Get()
    {
        return Ok(Tasks.Select(CreateTaskResponse));
    }

    [HttpPost]
    public ActionResult<TaskResponse> Post([FromBody] TaskRequest request)
    {
        var task = new TaskModel
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            IsCompleted = request.IsCompleted
        };

        Tasks.Add(task);

        return Ok(CreateTaskResponse(task));
    }

    [HttpPut("{id:guid}")]
    public ActionResult<TaskResponse> Put(Guid id, [FromBody] TaskRequest request)
    {
        var task = Tasks.FirstOrDefault(t => t.Id == id);

        if (task == null) return NotFound();

        task.Name = request.Name;
        task.IsCompleted = request.IsCompleted;

        return Ok(CreateTaskResponse(task));
    }

    [HttpDelete("{id:guid}")]
    public ActionResult Delete(Guid id)
    {
        var task = Tasks.FirstOrDefault(t => t.Id == id);

        if (task == null) return NotFound();

        Tasks.Remove(task);

        return Ok(CreateTaskResponse(task));
    }

    private static TaskResponse CreateTaskResponse(TaskModel task) => new TaskResponse
    {
        Id = task.Id,
        Name = task.Name,
        IsCompleted = task.IsCompleted
    };

    private record TaskModel
    {
        public Guid Id { get; init; }

        public string Name { get; set; } = null!;

        public bool IsCompleted { get; set; }
    }
}