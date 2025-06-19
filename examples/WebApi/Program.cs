using CarespaceSDK;
using CarespaceSDK.Extensions;
using CarespaceSDK.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCarespaceSDKForDevelopment();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/health", async (CarespaceClient carespace) =>
{
    var isHealthy = await carespace.HealthCheckAsync();
    return Results.Ok(new { healthy = isHealthy, timestamp = DateTime.UtcNow });
});

app.MapGet("/users", async (CarespaceClient carespace, int page = 1, int limit = 20, string? search = null) =>
{
    var users = await carespace.Users.GetUsersAsync(page, limit, search);
    return Results.Ok(users);
});

app.MapGet("/users/{id}", async (CarespaceClient carespace, string id) =>
{
    try
    {
        var user = await carespace.Users.GetUserAsync(id);
        return Results.Ok(user);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: 404);
    }
});

app.MapPost("/users", async (CarespaceClient carespace, CreateUserRequest request) =>
{
    try
    {
        var user = await carespace.Users.CreateUserAsync(request);
        return Results.Created($"/users/{user.Data?.Id}", user);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: 400);
    }
});

app.MapGet("/clients", async (CarespaceClient carespace, int page = 1, int limit = 20, string? search = null) =>
{
    var clients = await carespace.Clients.GetClientsAsync(page, limit, search);
    return Results.Ok(clients);
});

app.MapGet("/clients/{id}", async (CarespaceClient carespace, string id) =>
{
    try
    {
        var client = await carespace.Clients.GetClientAsync(id);
        return Results.Ok(client);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: 404);
    }
});

app.MapPost("/clients", async (CarespaceClient carespace, CreateClientRequest request) =>
{
    try
    {
        var client = await carespace.Clients.CreateClientAsync(request);
        return Results.Created($"/clients/{client.Data?.Id}", client);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: 400);
    }
});

app.MapGet("/clients/{id}/stats", async (CarespaceClient carespace, string id) =>
{
    try
    {
        var stats = await carespace.Clients.GetClientStatsAsync(id);
        return Results.Ok(stats);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: 404);
    }
});

app.MapGet("/programs", async (CarespaceClient carespace, int page = 1, int limit = 20, string? search = null, ProgramCategory? category = null) =>
{
    var programs = await carespace.Programs.GetProgramsAsync(page, limit, search, category);
    return Results.Ok(programs);
});

app.MapGet("/programs/{id}", async (CarespaceClient carespace, string id) =>
{
    try
    {
        var program = await carespace.Programs.GetProgramAsync(id);
        return Results.Ok(program);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: 404);
    }
});

app.MapPost("/programs", async (CarespaceClient carespace, CreateProgramRequest request) =>
{
    try
    {
        var program = await carespace.Programs.CreateProgramAsync(request);
        return Results.Created($"/programs/{program.Data?.Id}", program);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: 400);
    }
});

app.MapGet("/programs/{id}/exercises", async (CarespaceClient carespace, string id, int page = 1, int limit = 20) =>
{
    try
    {
        var exercises = await carespace.Programs.GetProgramExercisesAsync(id, page, limit);
        return Results.Ok(exercises);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: 404);
    }
});

app.MapPost("/programs/{programId}/exercises", async (CarespaceClient carespace, string programId, CreateExerciseRequest request) =>
{
    try
    {
        var exercise = await carespace.Programs.AddExerciseToProgramAsync(programId, request);
        return Results.Created($"/programs/{programId}/exercises/{exercise.Data?.Id}", exercise);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: 400);
    }
});

app.Run();