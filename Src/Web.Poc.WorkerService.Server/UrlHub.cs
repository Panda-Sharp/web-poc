using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Poc.Application.Contracts;

namespace Web.Poc.WorkerService.Server;

public class UrlHub : Hub<IUrl>
{
    public async Task SendUrlToClient(IEnumerable<string> urls)
    {
        await Clients.All.AddUrls(urls);
    }
}
