namespace Web.Poc.Application.Services.UrlDowload;

public class UrlMessage
{
    public UrlMessage()
    {
    }

    public UrlMessage(string redisId, string url, string status)
    {
        RedisId = redisId;
        Url = url;
        Status = status;
    }

    public string RedisId { get; set; }

    public string Url { get; set; }

    public string Status { get; set; }
}
