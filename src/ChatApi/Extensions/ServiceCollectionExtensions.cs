using Microsoft.Extensions.Options;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;

namespace ChatApi;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSemanticKernelWithChatCompletionsAndEmbeddingGeneration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<OpenAiOptions>(configuration.GetSection("OpenAi"));

        services.AddScoped(sp =>
        {
            var options = sp.GetRequiredService<IOptionsMonitor<OpenAiOptions>>().CurrentValue;

            var factory = sp.GetRequiredService<IHttpClientFactory>();

            var builder = Kernel.CreateBuilder();
            builder.AddAzureOpenAIChatCompletion(
                options.ChatModelName,
                options.ApiEndpoint,
                options.ApiKey,
                httpClient: factory.CreateClient()); // workaround for tracing requests using Fiddler

            builder.Services.AddLogging();

            var kernel = builder.Build();
            return kernel;
        });

        return services;
    }

    public static IServiceCollection AddKernelMemory(
            this IServiceCollection services,
            IConfiguration configuration)
    {
        services.AddSingleton(sp =>
        {
            var azureOpenAITextConfig = new AzureOpenAIConfig();
            var azureOpenAIEmbeddingConfig = new AzureOpenAIConfig();
            var azureAISearchConfig = new AzureAISearchConfig();

            configuration
                .BindSection("KernelMemory:Services:AzureOpenAIText", azureOpenAITextConfig)
                .BindSection("KernelMemory:Services:AzureOpenAIEmbedding", azureOpenAIEmbeddingConfig)
                .BindSection("KernelMemory:Services:AzureAISearch", azureAISearchConfig);

            var factory = sp.GetRequiredService<IHttpClientFactory>();

            var memory = new KernelMemoryBuilder()
                            .WithAzureOpenAITextEmbeddingGeneration(
                                config: azureOpenAIEmbeddingConfig,
                                httpClient: factory.CreateClient())
                            .WithAzureOpenAITextGeneration(
                                config: azureOpenAITextConfig,
                                httpClient: factory.CreateClient())
                            .WithAzureAISearchMemoryDb(azureAISearchConfig)
                            .Build<MemoryServerless>();

            return memory;
        });

        return services;
    }
}
