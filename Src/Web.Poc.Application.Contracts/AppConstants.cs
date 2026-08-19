namespace Web.Poc.Application.Contracts;

public static class AppConstants
{
    public static string HubConnection => $"https://localhost:5001{HubPath}";

    public static string HubPath => "/hubs/url";

    public static string UrlSentEvent => nameof(IUrl.OnAddUrls);
}