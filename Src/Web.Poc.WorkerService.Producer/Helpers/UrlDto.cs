using CsvHelper.Configuration.Attributes;

namespace Web.Poc.WorkerService.Producer.Helpers;

public class UrlDto
{
    [Index(0)]
    public string Url { get; set; } = string.Empty;
}