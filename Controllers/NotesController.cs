using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notes.Contracts;
using Notes.DataAccess;

namespace Notes.Controllers;

[ApiController]
[Route("[controller]")]
public class NotesController : ControllerBase
{
    private readonly NotesDbContext _context;

    public NotesController(NotesDbContext dbContext)
    {
        _context = dbContext;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNoteRequest request, CancellationToken ct)
    {
        var note = new Note(request.Title, request.Description);

        await _context.Notes.AddAsync(note, ct);
        await _context.SaveChangesAsync(ct);

        return Ok(note);
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetNotesRequest request, CancellationToken ct)
    {
        var notesQuery = _context.Notes.AsQueryable();

        
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            notesQuery = notesQuery.Where(n => n.Title.ToLower().Contains(request.Search.ToLower()));
        }
        
        Expression<Func<Note, object>> selectorKey = request.SortItem?.ToLower() switch 
        {
            "title" => note => note.Title,
            "createdat" or "date" => note => note.CreatedAt,
            _ => note => note.CreatedAt // default
        };

        
        if (request.SortOrder?.ToLower() == "desc")
        {
            notesQuery = notesQuery.OrderByDescending(selectorKey);
        }
        else
        {
            notesQuery = notesQuery.OrderBy(selectorKey);
        }

        var noteDtos = await notesQuery
            .Select(n => new NoteDto(n.Id, n.Title, n.Description, n.CreatedAt))
            .ToListAsync(ct);

        return Ok(new GetNotesResponse(noteDtos));
    }
}