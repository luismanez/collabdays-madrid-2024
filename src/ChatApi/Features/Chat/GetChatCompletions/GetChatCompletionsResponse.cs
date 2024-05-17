namespace ChatApi;

public class GetChatCompletionsResponse
{
    public string ChatAnswer { get; set; } = default!;
    public int TokenTotalUsage { get; set; }
}