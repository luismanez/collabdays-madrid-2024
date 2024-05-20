using Microsoft.Extensions.Options;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;

namespace ChatApi;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSemanticKernelWithChatCompletionsAndEmbeddingGeneration(
        this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();

        var openAiOptions = serviceProvider.GetRequiredService<IOptions<OpenAiOptions>>()!.Value;

        var kernelBuilder = services.AddKernel();
        kernelBuilder.Services
            .AddAzureOpenAIChatCompletion(
                endpoint: openAiOptions.ApiEndpoint,
                deploymentName: openAiOptions.ChatModelName,
                apiKey: openAiOptions.ApiKey)
            .AddAzureOpenAITextEmbeddingGeneration(
                endpoint: openAiOptions.ApiEndpoint,
                deploymentName: openAiOptions.EmbeddingsModelName,
                apiKey: openAiOptions.ApiKey
            );

        return services;
    }

    public static IServiceCollection AddKernelMemory(
            this IServiceCollection services,
            IConfiguration configuration)
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
}
