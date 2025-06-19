using CarespaceSDK.Models;

namespace CarespaceSDK.Api;

public interface IProgramsApi
{
    Task<PaginatedResponse<Program>> GetProgramsAsync(
        int page = 1, 
        int limit = 20, 
        string? search = null, 
        ProgramCategory? category = null, 
        ProgramDifficulty? difficulty = null,
        bool? isTemplate = null,
        bool? isPublic = null,
        string? creatorId = null,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<Program>> GetProgramAsync(string programId, CancellationToken cancellationToken = default);
    Task<ApiResponse<Program>> CreateProgramAsync(CreateProgramRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<Program>> UpdateProgramAsync(string programId, UpdateProgramRequest request, CancellationToken cancellationToken = default);
    Task DeleteProgramAsync(string programId, CancellationToken cancellationToken = default);
    
    Task<PaginatedResponse<Exercise>> GetProgramExercisesAsync(string programId, int page = 1, int limit = 20, CancellationToken cancellationToken = default);
    Task<ApiResponse<Exercise>> AddExerciseToProgramAsync(string programId, CreateExerciseRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<Exercise>> UpdateProgramExerciseAsync(string programId, string exerciseId, CreateExerciseRequest request, CancellationToken cancellationToken = default);
    Task RemoveExerciseFromProgramAsync(string programId, string exerciseId, CancellationToken cancellationToken = default);
    
    Task<ApiResponse<Program>> DuplicateProgramAsync(string programId, string? newName = null, CancellationToken cancellationToken = default);
    Task<ApiResponse<Program>> CreateProgramFromTemplateAsync(string templateId, string? name = null, CancellationToken cancellationToken = default);
}