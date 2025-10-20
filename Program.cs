using Notes.DataAccess;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddScoped<NotesDbContext>();

var app = builder.Build();
using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<NotesDbContext>();
await dbContext.Database.EnsureCreatedAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();

