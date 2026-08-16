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

	public async Task DownloaFile(string url)
	{
		string cwd = Directory.GetCurrentDirectory();
		var dirPath = Path.Combine(cwd, "Downloads");
		if (!Directory.Exists(dirPath))
		{
			Directory.CreateDirectory(dirPath);
		}

		HttpClient _httpClient = _httpClientFactory.CreateClient();

		try
		{
			HttpResponseMessage response = await _httpClient.GetAsync(url);
			//response.EnsureSuccessStatusCode();
			var responseContent = await response.Content.ReadAsByteArrayAsync();
			if (responseContent == null)
			{
				return;
			}

			var extension = Path.GetExtension(url);
			if (string.IsNullOrEmpty(extension))
			{
				extension = "html";
			}

			var now = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss");
			var filePath = Path.Combine(dirPath, $"{now}-file.{extension}");
			await File.WriteAllBytesAsync(filePath, responseContent);
		}
		catch (HttpRequestException e)
		{
			Console.WriteLine("Message :{0} ", e.Message);
		}
	}
}
