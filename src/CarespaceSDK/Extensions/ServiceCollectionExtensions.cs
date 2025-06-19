using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CarespaceSDK.Api;
using CarespaceSDK.Configuration;
using CarespaceSDK.Http;

namespace CarespaceSDK.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCarespaceSDK(
        this IServiceCollection services, 
        CarespaceConfiguration configuration)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));
        
        configuration.Validate();
        
        services.AddSingleton(configuration);
        services.AddHttpClient<IHttpClient, CarespaceHttpClient>();
        
        services.AddScoped<IAuthApi, AuthApi>();
        services.AddScoped<IUsersApi, UsersApi>();
        services.AddScoped<IClientsApi, ClientsApi>();
        services.AddScoped<IProgramsApi, ProgramsApi>();
        services.AddScoped<CarespaceClient>();
        
        return services;
    }

    public static IServiceCollection AddCarespaceSDK(
        this IServiceCollection services, 
        Action<CarespaceConfiguration> configureOptions)
    {
        var configuration = new CarespaceConfiguration();
        configureOptions(configuration);
        
        return services.AddCarespaceSDK(configuration);
    }

    public static IServiceCollection AddCarespaceSDK(
        this IServiceCollection services, 
        string apiKey, 
        string? baseUrl = null)
    {
        var configuration = new CarespaceConfiguration
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? "https://api-dev.carespace.ai"
        };
        
        return services.AddCarespaceSDK(configuration);
    }

    public static IServiceCollection AddCarespaceSDKForDevelopment(
        this IServiceCollection services, 
        string? apiKey = null)
    {
        return services.AddCarespaceSDK(CarespaceConfiguration.ForDevelopment(apiKey));
    }

    public static IServiceCollection AddCarespaceSDKForProduction(
        this IServiceCollection services, 
        string apiKey)
    {
        return services.AddCarespaceSDK(CarespaceConfiguration.ForProduction(apiKey));
    }

    public static IServiceCollection AddCarespaceSDKForStaging(
        this IServiceCollection services, 
        string apiKey)
    {
        return services.AddCarespaceSDK(CarespaceConfiguration.ForStaging(apiKey));
    }
}