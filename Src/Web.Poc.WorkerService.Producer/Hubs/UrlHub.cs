using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Poc.Application.Contracts;

namespace Web.Poc.WorkerService.Producer.Hubs;

public class UrlHub : Hub<IUrl>
{
    public static bool IsConnected;

    public async Task SendUrlToClient(IEnumerable<string> urls)
    {
        await Clients.All.OnAddUrls(urls);
    }

    public override async Task OnConnectedAsync()
    {
        IsConnected = true;
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        IsConnected = false;
        await base.OnDisconnectedAsync(exception);
    }
}
