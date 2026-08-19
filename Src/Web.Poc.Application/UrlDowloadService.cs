using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Web.Poc.Application.Contracts;

namespace Web.Poc.Application;

public class UrlDowloadService : IUrlDowloadService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<UrlDowloadService> _logger;

    public UrlDowloadService(
        IHttpClientFactory httpClientFactory,
        ILogger<UrlDowloadService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task DownloaFile(Uri uri)
    {
        var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
        string cwd = Directory.GetCurrentDirectory();
        var dirPath = Path.Combine(cwd, "Downloads", today);
        Directory.CreateDirectory(dirPath);

        HttpClient _httpClient = _httpClientFactory.CreateClient();

        try
        {
            var extension = Path.GetExtension(uri.AbsoluteUri);
            if (string.IsNullOrEmpty(extension))
            {
                extension = "html";
            }

            var now = DateTime.UtcNow.ToString("HH-mm-ss-ff");
            var filePath = Path.Combine(dirPath, $"{now}-{uri.Host}.{extension}");

            using var downloadStream = await _httpClient.GetStreamAsync(uri);
            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);

            await downloadStream.CopyToAsync(fileStream);
            await fileStream.FlushAsync();
            fileStream.Close();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred executing task.");
        }
    }
}
