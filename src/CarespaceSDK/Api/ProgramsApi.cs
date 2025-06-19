using CarespaceSDK.Http;
using CarespaceSDK.Models;

namespace CarespaceSDK.Api;

public class ProgramsApi : IProgramsApi
{
    private readonly IHttpClient _httpClient;

    public ProgramsApi(IHttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<PaginatedResponse<Program>> GetProgramsAsync(
        int page = 1, 
        int limit = 20, 
        string? search = null, 
        ProgramCategory? category = null, 
        ProgramDifficulty? difficulty = null,
        bool? isTemplate = null,
        bool? isPublic = null,
        string? creatorId = null,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string>();
        queryParams.Add($"page={page}");
        queryParams.Add($"limit={limit}");
        
        if (!string.IsNullOrEmpty(search))
            queryParams.Add($"search={Uri.EscapeDataString(search)}");
        
        if (category.HasValue)
            queryParams.Add($"category={category.Value}");
        
        if (difficulty.HasValue)
            queryParams.Add($"difficulty={difficulty.Value}");
        
        if (isTemplate.HasValue)
            queryParams.Add($"is_template={isTemplate.Value}");
        
        if (isPublic.HasValue)
            queryParams.Add($"is_public={isPublic.Value}");
        
        if (!string.IsNullOrEmpty(creatorId))
            queryParams.Add($"creator_id={creatorId}");

        var query = string.Join("&", queryParams);
        var endpoint = $"/programs?{query}";

        return await _httpClient.GetAsync<PaginatedResponse<Program>>(endpoint, cancellationToken);
    }

    public async Task<ApiResponse<Program>> GetProgramAsync(string programId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(programId)) throw new ArgumentException("Program ID is required", nameof(programId));
        return await _httpClient.GetAsync<ApiResponse<Program>>($"/programs/{programId}", cancellationToken);
    }

    public async Task<ApiResponse<Program>> CreateProgramAsync(CreateProgramRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        return await _httpClient.PostAsync<ApiResponse<Program>>("/programs", request, cancellationToken);
    }

    public async Task<ApiResponse<Program>> UpdateProgramAsync(string programId, UpdateProgramRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(programId)) throw new ArgumentException("Program ID is required", nameof(programId));
        if (request == null) throw new ArgumentNullException(nameof(request));
        return await _httpClient.PutAsync<ApiResponse<Program>>($"/programs/{programId}", request, cancellationToken);
    }

    public async Task DeleteProgramAsync(string programId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(programId)) throw new ArgumentException("Program ID is required", nameof(programId));
        await _httpClient.DeleteAsync($"/programs/{programId}", cancellationToken);
    }

    public async Task<PaginatedResponse<Exercise>> GetProgramExercisesAsync(string programId, int page = 1, int limit = 20, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(programId)) throw new ArgumentException("Program ID is required", nameof(programId));
        
        var queryParams = new List<string>();
        queryParams.Add($"page={page}");
        queryParams.Add($"limit={limit}");
        var query = string.Join("&", queryParams);
        
        return await _httpClient.GetAsync<PaginatedResponse<Exercise>>($"/programs/{programId}/exercises?{query}", cancellationToken);
    }

    public async Task<ApiResponse<Exercise>> AddExerciseToProgramAsync(string programId, CreateExerciseRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(programId)) throw new ArgumentException("Program ID is required", nameof(programId));
        if (request == null) throw new ArgumentNullException(nameof(request));
        
        return await _httpClient.PostAsync<ApiResponse<Exercise>>($"/programs/{programId}/exercises", request, cancellationToken);
    }

    public async Task<ApiResponse<Exercise>> UpdateProgramExerciseAsync(string programId, string exerciseId, CreateExerciseRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(programId)) throw new ArgumentException("Program ID is required", nameof(programId));
        if (string.IsNullOrWhiteSpace(exerciseId)) throw new ArgumentException("Exercise ID is required", nameof(exerciseId));
        if (request == null) throw new ArgumentNullException(nameof(request));
        
        return await _httpClient.PutAsync<ApiResponse<Exercise>>($"/programs/{programId}/exercises/{exerciseId}", request, cancellationToken);
    }

    public async Task RemoveExerciseFromProgramAsync(string programId, string exerciseId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(programId)) throw new ArgumentException("Program ID is required", nameof(programId));
        if (string.IsNullOrWhiteSpace(exerciseId)) throw new ArgumentException("Exercise ID is required", nameof(exerciseId));
        
        await _httpClient.DeleteAsync($"/programs/{programId}/exercises/{exerciseId}", cancellationToken);
    }

    public async Task<ApiResponse<Program>> DuplicateProgramAsync(string programId, string? newName = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(programId)) throw new ArgumentException("Program ID is required", nameof(programId));
        
        var request = newName != null ? new { name = newName } : null;
        return await _httpClient.PostAsync<ApiResponse<Program>>($"/programs/{programId}/duplicate", request, cancellationToken);
    }

    public async Task<ApiResponse<Program>> CreateProgramFromTemplateAsync(string templateId, string? name = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(templateId)) throw new ArgumentException("Template ID is required", nameof(templateId));
        
        var request = name != null ? new { name } : null;
        return await _httpClient.PostAsync<ApiResponse<Program>>($"/programs/from-template/{templateId}", request, cancellationToken);
    }
}