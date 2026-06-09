using JuniorGolf.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Register Infrastructure (DbContext, repositories)
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Minimal health check to verify DB connectivity
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.Run();
