using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Poc.Application.Services.Redis;
using Web.Poc.Application.Services.UrlDowload;
using Web.Poc.Domain;
using Web.Poc.WorkerService.Producer.Dtos;
using Web.Poc.WorkerService.Producer.Helpers;

namespace Web.Poc.WorkerService.Producer.Services;

public class ProducerService : IProducerService
{
    private readonly IRedisService _redisService;
    private readonly ILogger<ProducerService> _logger;
    private readonly string _urlFileName;

    public ProducerService(
        IConfiguration configuration,
        IRedisService redisService,
        ILogger<ProducerService> logger)
    {
        _redisService = redisService;
        _logger = logger;

        _urlFileName = configuration["UrlFileName"] ?? string.Empty;
    }

    public IAsyncEnumerable<UrlMessage> ConsumeAsync(string key)
    {
        return _redisService.StreamReadGroupAsync(key);
    }

    public async Task PublishAsync()
    {
        var page = 0;
        while (true)
        {
            var urls = UrlsHelper.GetFromCsv(page, _urlFileName);
            if (!urls.Any())
            {
                break;
            }

            _logger.LogInformation("Sending urls...: {url}", urls.Count());
            var urlMessageDtos = urls.Select(url => new UrlMessageDto(url, "new"));
            var payload = urlMessageDtos.Select(x => x.ToUrlMessage());
            await _redisService.StreamAddAsync(UrlAppConstants.NewUrlsStreamKey, payload);
            _logger.LogInformation("...urls Sent: {url}", urls.Count());
            await Task.Delay(500);

            page++;
        }
    }
}
