using Microsoft.AspNetCore.Mvc;

namespace Notes.Contracts;

public record GetNotesResponse(List<NoteDto> Notes) : IActionResult
{
    private IActionResult _actionResultImplementation;
    public Task ExecuteResultAsync(ActionContext context)
    {
        return _actionResultImplementation.ExecuteResultAsync(context);
    }
}

public record NoteDto(Guid Id, string Title, string Description, DateTime CreatedAt);