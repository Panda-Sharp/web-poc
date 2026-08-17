namespace Web.Poc.Application.Contracts;

public static class AppConstants
{
    public static string HubConsumerUrl => $"https://localhost:5001{HubProducerUrl}";

    public static string HubProducerUrl => "/hubs/url";

    public static string UrlSentEvent => nameof(IUrl.OnAddUrls);
}