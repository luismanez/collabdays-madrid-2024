using FastEndpoints;

namespace ChatApi;

public class GetChatCompletionsEndpoint : Endpoint<GetChatCompletionsRequest, GetChatCompletionsResponse>
{
    public override void Configure()
    {
        Post("/api/chat");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetChatCompletionsRequest req, CancellationToken ct)
    {
        var response = new GetChatCompletionsResponse
        {
            ChatAnswer = "Hello, how can I help you?",
            TokenTotalUsage = 10
        };

        await SendAsync(response, cancellation: ct);
    }
}