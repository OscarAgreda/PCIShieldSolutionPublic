using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

namespace PCIShield.Api.Saga.SignalR;

public interface IPCIShieldAppApiSignalrHub
{
    Task JoinUserGroupConnection(string userId);
    Task OnConnectedAsync();
    Task OnDisconnectedAsync(Exception? exception);
    void Dispose();
    IHubCallerClients Clients { get; set; }
    HubCallerContext Context { get; set; }
    IGroupManager Groups { get; set; }
}