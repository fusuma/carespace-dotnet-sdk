using CarespaceSDK;
using CarespaceSDK.Configuration;
using CarespaceSDK.Models;
using Microsoft.Extensions.Logging;

namespace CarespaceSDK.Examples.ConsoleApp;

class Program
{
    static async Task Main(string[] args)
    {
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<Program>();

        try
        {
            await RunBasicExampleAsync(logger);
            await RunAdvancedExampleAsync(logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while running the examples");
        }
    }

    static async Task RunBasicExampleAsync(ILogger logger)
    {
        logger.LogInformation("=== Basic Carespace SDK Example ===");

        using var client = CarespaceClient.CreateForDevelopment();

        logger.LogInformation("Performing health check...");
        var isHealthy = await client.HealthCheckAsync();
        logger.LogInformation("API Health: {IsHealthy}", isHealthy ? "Healthy" : "Unhealthy");

        logger.LogInformation("Getting users...");
        var users = await client.QuickGetUsersAsync(limit: 5);
        logger.LogInformation("Found {UserCount} users", users.Data?.Count ?? 0);

        if (users.Data?.Any() == true)
        {
            foreach (var user in users.Data.Take(3))
            {
                logger.LogInformation("User: {Name} ({Email}) - Role: {Role}", 
                    user.Name, user.Email, user.Role);
            }
        }

        logger.LogInformation("Getting clients...");
        var clients = await client.QuickGetClientsAsync(limit: 5);
        logger.LogInformation("Found {ClientCount} clients", clients.Data?.Count ?? 0);

        logger.LogInformation("Getting programs...");
        var programs = await client.QuickGetProgramsAsync(limit: 5, category: ProgramCategory.Rehabilitation);
        logger.LogInformation("Found {ProgramCount} rehabilitation programs", programs.Data?.Count ?? 0);

        logger.LogInformation("Basic example completed successfully!");
    }

    static async Task RunAdvancedExampleAsync(ILogger logger)
    {
        logger.LogInformation("\n=== Advanced Carespace SDK Example ===");

        var configuration = CarespaceConfiguration.ForDevelopment();
        configuration.EnableLogging = true;
        configuration.LogLevel = LogLevel.Debug;

        using var httpLogger = LoggerFactory.Create(builder => builder.AddConsole())
            .CreateLogger<CarespaceSDK.Http.CarespaceHttpClient>();

        using var client = new CarespaceClient(configuration, httpLogger);

        try
        {
            logger.LogInformation("Creating a new user...");
            var newUser = new CreateUserRequest
            {
                Email = $"test.user.{Guid.NewGuid():N}@example.com",
                Name = "Test User",
                FirstName = "Test",
                LastName = "User",
                Password = "SecurePassword123!",
                Role = UserRole.Patient,
                Profile = new UserProfile
                {
                    Phone = "+1-555-0123",
                    DateOfBirth = DateTime.Parse("1990-01-01"),
                    Address = new Address
                    {
                        Street = "123 Main St",
                        City = "Anytown",
                        State = "CA",
                        PostalCode = "12345",
                        Country = "USA"
                    }
                }
            };

            var createdUserResponse = await client.Users.CreateUserAsync(newUser);
            if (createdUserResponse.Success && createdUserResponse.Data != null)
            {
                logger.LogInformation("Created user: {UserId} - {Name}", 
                    createdUserResponse.Data.Id, createdUserResponse.Data.Name);

                logger.LogInformation("Creating a new client...");
                var newClient = new CreateClientRequest
                {
                    Name = "Jane Doe",
                    Email = $"jane.doe.{Guid.NewGuid():N}@example.com",
                    Phone = "+1-555-0456",
                    DateOfBirth = DateTime.Parse("1985-05-15"),
                    Gender = "Female",
                    MedicalInfo = new MedicalInfo
                    {
                        Allergies = new[] { "Peanuts", "Shellfish" },
                        Conditions = new[] { "Hypertension" },
                        Notes = "Patient recovering from knee surgery"
                    }
                };

                var createdClientResponse = await client.Clients.CreateClientAsync(newClient);
                if (createdClientResponse.Success && createdClientResponse.Data != null)
                {
                    logger.LogInformation("Created client: {ClientId} - {Name}", 
                        createdClientResponse.Data.Id, createdClientResponse.Data.Name);

                    logger.LogInformation("Creating a rehabilitation program...");
                    var newProgram = new CreateProgramRequest
                    {
                        Name = "Post-Surgery Knee Rehabilitation",
                        Description = "Comprehensive rehabilitation program for post-surgical knee recovery",
                        Category = ProgramCategory.Rehabilitation,
                        Difficulty = ProgramDifficulty.Beginner,
                        Duration = 30,
                        IsTemplate = false,
                        IsPublic = true,
                        Tags = new[] { "knee", "surgery", "rehabilitation", "beginner" }
                    };

                    var createdProgramResponse = await client.Programs.CreateProgramAsync(newProgram);
                    if (createdProgramResponse.Success && createdProgramResponse.Data != null)
                    {
                        logger.LogInformation("Created program: {ProgramId} - {Name}", 
                            createdProgramResponse.Data.Id, createdProgramResponse.Data.Name);

                        logger.LogInformation("Adding exercises to the program...");
                        var exercises = new[]
                        {
                            new CreateExerciseRequest
                            {
                                Name = "Knee Flexion",
                                Description = "Gentle knee bending exercise",
                                Instructions = "Slowly bend your knee while lying down, hold for 5 seconds",
                                Duration = 60,
                                Repetitions = 10,
                                Sets = 3,
                                RestTime = 30,
                                BodyParts = new[] { "Knee", "Quadriceps" },
                                Equipment = new[] { "Exercise mat" },
                                Order = 1
                            },
                            new CreateExerciseRequest
                            {
                                Name = "Heel Slides",
                                Description = "Slide your heel towards your buttocks",
                                Instructions = "While lying down, slide your heel towards your buttocks and back",
                                Duration = 45,
                                Repetitions = 15,
                                Sets = 2,
                                RestTime = 20,
                                BodyParts = new[] { "Knee", "Hamstring" },
                                Equipment = new[] { "Exercise mat" },
                                Order = 2
                            }
                        };

                        foreach (var exercise in exercises)
                        {
                            var exerciseResponse = await client.Programs.AddExerciseToProgramAsync(
                                createdProgramResponse.Data.Id, exercise);
                            
                            if (exerciseResponse.Success && exerciseResponse.Data != null)
                            {
                                logger.LogInformation("Added exercise: {ExerciseName}", exercise.Name);
                            }
                        }

                        logger.LogInformation("Assigning program to client...");
                        await client.Clients.AssignProgramAsync(
                            createdClientResponse.Data.Id, 
                            createdProgramResponse.Data.Id);
                        
                        logger.LogInformation("Program assigned successfully!");

                        logger.LogInformation("Getting client stats...");
                        var clientStats = await client.Clients.GetClientStatsAsync(createdClientResponse.Data.Id);
                        if (clientStats.Success && clientStats.Data != null)
                        {
                            logger.LogInformation("Client stats - Total sessions: {TotalSessions}, Completion rate: {CompletionRate}%", 
                                clientStats.Data.TotalSessions, clientStats.Data.CompletionRate * 100);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in advanced example");
        }

        logger.LogInformation("Advanced example completed!");
    }
}