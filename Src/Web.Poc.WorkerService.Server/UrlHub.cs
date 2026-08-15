using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Web.Poc.Application.Contracts;

namespace Web.Poc.WorkerService.Server;

public class UrlHub : Hub<IUrl>
{
    public async Task SendUrlToClient(string url)
    {
        await Clients.All.ShowUrl(url);
    }
}
