using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Web.Poc.WorkerService.Producer.Services;

namespace Web.Poc.WorkerService.Producer.Controllers;

[Route("[controller]")]
[ApiController]
public class ApiController : ControllerBase
{
    private readonly IProducerService _producerService;
    private readonly ILogger<ApiController> _logger;

    public ApiController(
        IProducerService producerService,
        ILogger<ApiController> logger)
    {
        _producerService = producerService;
        _logger = logger;
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetStatusAsync()
    {
        return Ok("Producer is up");
    }

    /// <summary>
    /// start to publish urls from csv
    /// </summary>
    /// <returns></returns>
    [HttpPost("publish")]
    public async Task<IActionResult> Publish() // [FromBody] UrlMessageDto message
    {
        await _producerService.PublishAsync();

        return Ok();
    }

    /// <summary>
    /// read values from stream by key
    /// <example>urls:new</example>
    /// </summary>
    /// <param name="key"><example>urls:new</example></param>
    /// <returns></returns>
    [HttpGet("read-stream/{key}")]
    public async Task<IActionResult> ConsumeAsync([FromRoute] string key)
    {
        var result = _producerService.ConsumeAsync(key);

        return Ok(result);
    }
}
