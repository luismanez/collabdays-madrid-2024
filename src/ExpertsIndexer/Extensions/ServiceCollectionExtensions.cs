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

    public static IServiceCollection AddMicrosoftGraphForApplication(
        this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddSingleton<GraphServiceClient, GraphServiceClient>(serviceProvider =>
        {
            string[] graphDefaultScopes = ["https://graph.microsoft.com/.default"];

            var azureAdOptions = serviceProvider.GetService<IOptions<AzureAdOptions>>()!.Value;

            var clientSecretCredential = new ClientSecretCredential(
               azureAdOptions.TenantId,
               azureAdOptions.ClientId,
               azureAdOptions.ClientSecret);

            return new GraphServiceClient(clientSecretCredential, graphDefaultScopes);
        });
    }
}
