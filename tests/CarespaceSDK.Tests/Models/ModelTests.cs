using Xunit;
using FluentAssertions;
using CarespaceSDK.Models;

namespace CarespaceSDK.Tests.Models;

public class ModelTests
{
    [Fact]
    public void LoginRequest_ShouldHaveCorrectProperties()
    {
        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "password123"
        };

        loginRequest.Email.Should().Be("test@example.com");
        loginRequest.Password.Should().Be("password123");
    }

    [Fact]
    public void User_ShouldHaveCorrectProperties()
    {
        var user = new User
        {
            Id = "user-123",
            Email = "test@example.com",
            Name = "Test User",
            FirstName = "Test",
            LastName = "User",
            Role = UserRole.Patient,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        user.Id.Should().Be("user-123");
        user.Email.Should().Be("test@example.com");
        user.Name.Should().Be("Test User");
        user.Role.Should().Be(UserRole.Patient);
        user.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Client_ShouldHaveCorrectProperties()
    {
        var client = new Client
        {
            Id = "client-123",
            Name = "John Doe",
            Email = "john@example.com",
            Phone = "+1-555-0123",
            IsActive = true,
            TotalSessions = 10,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        client.Id.Should().Be("client-123");
        client.Name.Should().Be("John Doe");
        client.Email.Should().Be("john@example.com");
        client.IsActive.Should().BeTrue();
        client.TotalSessions.Should().Be(10);
    }

    [Fact]
    public void Program_ShouldHaveCorrectProperties()
    {
        var program = new Program
        {
            Id = "program-123",
            Name = "Test Program",
            Description = "A test program",
            Category = ProgramCategory.Rehabilitation,
            Difficulty = ProgramDifficulty.Beginner,
            Duration = 30,
            IsTemplate = false,
            IsPublic = true,
            CreatorId = "user-123",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        program.Id.Should().Be("program-123");
        program.Name.Should().Be("Test Program");
        program.Category.Should().Be(ProgramCategory.Rehabilitation);
        program.Difficulty.Should().Be(ProgramDifficulty.Beginner);
        program.Duration.Should().Be(30);
        program.IsTemplate.Should().BeFalse();
        program.IsPublic.Should().BeTrue();
    }

    [Fact]
    public void Exercise_ShouldHaveCorrectProperties()
    {
        var exercise = new Exercise
        {
            Id = "exercise-123",
            Name = "Push ups",
            Description = "Upper body exercise",
            Duration = 60,
            Repetitions = 10,
            Sets = 3,
            RestTime = 30,
            Order = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        exercise.Id.Should().Be("exercise-123");
        exercise.Name.Should().Be("Push ups");
        exercise.Duration.Should().Be(60);
        exercise.Repetitions.Should().Be(10);
        exercise.Sets.Should().Be(3);
        exercise.Order.Should().Be(1);
    }

    [Fact]
    public void ApiResponse_ShouldHaveCorrectProperties()
    {
        var response = new ApiResponse<string>
        {
            Success = true,
            Data = "test data",
            Message = "Success"
        };

        response.Success.Should().BeTrue();
        response.Data.Should().Be("test data");
        response.Message.Should().Be("Success");
        response.Error.Should().BeNull();
    }

    [Fact]
    public void PaginatedResponse_ShouldHaveCorrectProperties()
    {
        var users = new List<User>
        {
            new User { Id = "1", Name = "User 1", Email = "user1@example.com", Role = UserRole.Patient },
            new User { Id = "2", Name = "User 2", Email = "user2@example.com", Role = UserRole.Provider }
        };

        var response = new PaginatedResponse<User>
        {
            Success = true,
            Data = users,
            Page = 1,
            Limit = 20,
            Total = 100,
            TotalPages = 5,
            HasNext = true,
            HasPrevious = false
        };

        response.Success.Should().BeTrue();
        response.Data.Should().HaveCount(2);
        response.Page.Should().Be(1);
        response.Limit.Should().Be(20);
        response.Total.Should().Be(100);
        response.TotalPages.Should().Be(5);
        response.HasNext.Should().BeTrue();
        response.HasPrevious.Should().BeFalse();
    }
}