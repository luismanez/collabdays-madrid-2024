using FastEndpoints;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Planning.Handlebars;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

namespace ChatApi;

public class GetChatCompletionsEndpoint : Endpoint<GetChatCompletionsRequest, GetChatCompletionsResponse>
{
    private readonly Kernel _kernel;

    public GetChatCompletionsEndpoint(Kernel kernel)
    {
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
        _kernel.CreatePluginFromFunctions("ExpertFinderPlugin", [expertFinderFunction]);

        var planner = new HandlebarsPlanner(new HandlebarsPlannerOptions() { AllowLoops = true });

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