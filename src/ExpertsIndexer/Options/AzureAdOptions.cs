namespace ExpertsIndexer;

public class AzureAdOptions
{
    public static readonly string SettingsSectionName = "AzureAd";

    public string ClientId { get; set; } = default!;
    public string ClientSecret { get; set; } = default!;
    public string TenantId { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
}
