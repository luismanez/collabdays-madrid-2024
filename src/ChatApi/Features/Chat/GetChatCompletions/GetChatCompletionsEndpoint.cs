using FastEndpoints;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning.Handlebars;

namespace ChatApi;

public class GetChatCompletionsEndpoint : Endpoint<GetChatCompletionsRequest, GetChatCompletionsResponse>
{
    private readonly Kernel _kernel;
    private readonly MemoryServerless _kernelMemory;

    public GetChatCompletionsEndpoint(
        Kernel kernel,
        MemoryServerless kernelMemory)
    {
        _kernelMemory = kernelMemory;
        _kernel = kernel;
    }

    public override void Configure()
    {
        Post("/api/chat");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetChatCompletionsRequest req, CancellationToken ct)
    {
        var expertFinderYaml = EmbeddedResource.Read("ExpertFinder.yaml");
        var expertFinderFunction = _kernel.CreateFunctionFromPromptYaml(expertFinderYaml);
        _kernel.ImportPluginFromFunctions("ExpertFinderPlugin", [expertFinderFunction]);

        var friendlyChatYaml = EmbeddedResource.Read("FriendlyChat.yaml");
        var friendlyChatFunction = _kernel.CreateFunctionFromPromptYaml(friendlyChatYaml);
        _kernel.ImportPluginFromFunctions("FriendlyChatPlugin", [friendlyChatFunction]);

        var plugin = new MemoryPlugin(_kernelMemory,
                                      waitForIngestionToComplete: true,
                                      defaultIndex: "CompanyExperts");

        _kernel.ImportPluginFromObject(plugin, "memory");

        var planner = new HandlebarsPlanner(
            new HandlebarsPlannerOptions()
            {
                AllowLoops = true,
                GetAdditionalPromptContext = () => Task.FromResult("If you cannot find any helper to achieve user's goal, just answer your knowledge (LLM).")
            });

        var plan = await planner.CreatePlanAsync(_kernel, req.Input!, cancellationToken: ct);

        var result = await plan.InvokeAsync(_kernel, cancellationToken: ct);

        var response = new GetChatCompletionsResponse
        {
            ChatAnswer = result,
            TokenTotalUsage = 10
        };

        await SendAsync(response, cancellation: ct);
    }
}