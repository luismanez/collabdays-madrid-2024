namespace ChatApi;

public class GetChatCompletionsResponse
{
    public string ChatAnswer { get; set; } = default!;
    public string UserQuery { get; set; } = default!;
}