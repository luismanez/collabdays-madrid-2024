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
        var azureOpenAiTextConfig = new AzureOpenAIConfig();
        configuration
                .BindSection("KernelMemory:Services:AzureOpenAIText", azureOpenAiTextConfig);

        services.AddScoped(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();

            var builder = Kernel.CreateBuilder();
            builder.AddAzureOpenAIChatCompletion(
                azureOpenAiTextConfig.Deployment,
                azureOpenAiTextConfig.Endpoint,
                azureOpenAiTextConfig.APIKey,
                httpClient: factory.CreateClient()); // workaround for tracing requests using Fiddler

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
            var azureOpenAiTextConfig = new AzureOpenAIConfig();
            var azureOpenAiEmbeddingConfig = new AzureOpenAIConfig();
            var azureAiSearchConfig = new AzureAISearchConfig();

            configuration
                .BindSection("KernelMemory:Services:AzureOpenAIText", azureOpenAiTextConfig)
                .BindSection("KernelMemory:Services:AzureOpenAIEmbedding", azureOpenAiEmbeddingConfig)
                .BindSection("KernelMemory:Services:AzureAISearch", azureAiSearchConfig);

            var factory = sp.GetRequiredService<IHttpClientFactory>();

            var kmBuilder = new KernelMemoryBuilder()
                            .WithAzureOpenAITextEmbeddingGeneration(
                                config: azureOpenAiEmbeddingConfig,
                                httpClient: factory.CreateClient())
                            .WithAzureOpenAITextGeneration(
                                config: azureOpenAiTextConfig,
                                httpClient: factory.CreateClient())
                            .WithAzureAISearchMemoryDb(azureAiSearchConfig);

            kmBuilder.Services.AddLogging(builder =>
            {
                builder.AddConsole();

                // builder.AddApplicationInsights(telemetryConfiguration =>
                // {
                //     // telemetryConfiguration.ConnectionString = configuration["ApplicationInsights:ConnectionString"];
                //     telemetryConfiguration.InstrumentationKey = configuration["ApplicationInsights:InstrumentationKey"];
                // }, loggerOptions =>
                // {
                //     //loggerOptions.FlushOnDispose = true;
                // });
            });

            var memory = kmBuilder.Build<MemoryServerless>();

            return memory;
        });

        return services;
    }
}
