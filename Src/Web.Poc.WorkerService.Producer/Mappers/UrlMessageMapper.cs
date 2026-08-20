using Riok.Mapperly.Abstractions;
using Web.Poc.Application.Services.UrlDowload;
using Web.Poc.WorkerService.Producer.Dtos;

[Mapper]
public static partial class UrlMessageMapper
{
    //[MapperIgnoreTarget(nameof(UrlMessage.RedisId))]
    public static partial UrlMessage ToUrlMessage(this UrlMessageDto car);
}