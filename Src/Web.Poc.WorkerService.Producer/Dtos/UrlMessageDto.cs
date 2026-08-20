namespace Web.Poc.WorkerService.Producer.Dtos;

public class UrlMessageDto
{
    public UrlMessageDto(string url, string status)
    {
        Url = url;
        Status = status;
    }

    public string RedisId { get; set; } = string.Empty;

    public string Url { get; set; }

    public string Status { get; set; }
}