using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Graph.Beta;
using Microsoft.KernelMemory;

namespace ExpertsIndexer;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationOptions(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.AddOptions<AzureAdOptions>()
            .Bind(configuration.GetSection(AzureAdOptions.SettingsSectionName));

        return services;
    }

    public static IServiceCollection AddKernelMemory(
        this IServiceCollection services, IConfiguration configuration)
    {
        var azureOpenAITextConfig = new AzureOpenAIConfig();
        var azureOpenAIEmbeddingConfig = new AzureOpenAIConfig();
        var azureAISearchConfig = new AzureAISearchConfig();

        configuration
            .BindSection("KernelMemory:Services:AzureOpenAIText", azureOpenAITextConfig)
            .BindSection("KernelMemory:Services:AzureOpenAIEmbedding", azureOpenAIEmbeddingConfig)
            .BindSection("KernelMemory:Services:AzureAISearch", azureAISearchConfig);

        var memory = new KernelMemoryBuilder()
                        .WithAzureOpenAITextEmbeddingGeneration(azureOpenAIEmbeddingConfig)
                        .WithAzureOpenAITextGeneration(azureOpenAITextConfig)
                        .WithAzureAISearchMemoryDb(azureAISearchConfig)
                        .Build<MemoryServerless>();

        services.AddSingleton(memory);

        return services;
    }

    /// <summary>
    /// MS Graph Profile beta endpoint does not support application permissions.
    /// We need to use a admin user account. This is not recommended for production (only for demo purposes)
    /// Permissions: https://learn.microsoft.com/en-us/graph/api/profile-get?view=graph-rest-beta&tabs=http#permissions
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    /// <seealso cref="https://learn.microsoft.com/en-us/graph/api/profile-get?view=graph-rest-beta&tabs=http#permissions"/>
    public static IServiceCollection AddMicrosoftGraphForAdminUser(
        this IServiceCollection services)
    {
        return services.AddSingleton<GraphServiceClient, GraphServiceClient>(serviceProvider =>
        {
            string[] graphDefaultScopes = ["https://graph.microsoft.com/.default"];

            var azureAdOptions = serviceProvider.GetService<IOptions<AzureAdOptions>>()!.Value;

            var userNamePasswordCredential = new UsernamePasswordCredential(
                azureAdOptions.UserName,
                azureAdOptions.Password,
                azureAdOptions.TenantId,
                azureAdOptions.ClientId);

            return new GraphServiceClient(userNamePasswordCredential, graphDefaultScopes);
        });
    }
}
