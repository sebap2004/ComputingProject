using Microsoft.AspNetCore.SignalR;

namespace chatapptestnotborken.Hubs;

public class ChatHub : Hub
{
    public async Task SendMessage(string user, string message, bool systemMessage)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message, systemMessage);
    }
}