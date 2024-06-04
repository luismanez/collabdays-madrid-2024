using FastEndpoints;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace ChatApi;

public class GetChatCompletionsEndpoint : Endpoint<GetChatCompletionsRequest, GetChatCompletionsResponse>
{
    private readonly Kernel _kernel;
    private readonly MemoryServerless _kernelMemory;
    private readonly ILogger<GetChatCompletionsEndpoint> _logger;

    public GetChatCompletionsEndpoint(
        Kernel kernel,
        MemoryServerless kernelMemory,
        ILogger<GetChatCompletionsEndpoint> logger)
    {
        _kernelMemory = kernelMemory;
        _logger = logger;
        _kernel = kernel;
    }

    public override void Configure()
    {
        Post("/api/chat");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetChatCompletionsRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Received chat request: {Input}", req.Input);

        var expertFinderYaml = EmbeddedResource.Read("ExpertFinder.yaml");
        var expertFinderFunction = _kernel.CreateFunctionFromPromptYaml(expertFinderYaml);
        _kernel.ImportPluginFromFunctions("ExpertFinderPlugin", [expertFinderFunction]); // Adding ExpertFinder plugin

        var plugin = new MemoryPlugin(_kernelMemory,
                                      waitForIngestionToComplete: true,
                                      defaultIndex: "CompanyExperts");

        _kernel.ImportPluginFromObject(plugin, "memory"); // Adding KernelMemory plugin

        var settings = new OpenAIPromptExecutionSettings
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };
        var result = await _kernel.InvokePromptAsync<string>(
            req.Input,
            new(settings),
            cancellationToken: ct);

        var response = new GetChatCompletionsResponse
        {
            UserQuery = req.Input,
            ChatAnswer = result!
        };

        await SendAsync(response, cancellation: ct);
    }
}