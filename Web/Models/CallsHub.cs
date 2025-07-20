using Microsoft.AspNetCore.SignalR;

namespace FNS.Main.Models;

public class CallsHub: Hub
{
    public async Task SendCall(string userName)
    {
        await Clients.All.SendAsync("ReceiveCall", userName);
    }
}
