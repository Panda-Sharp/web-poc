namespace Web.Poc.Application.Contracts;

public static class AppConstants
{
    public static string HubUrl => "https://localhost:5001/hubs/clock";

    public static string UrlSentEvent => nameof(IUrl.ShowUrl);
}