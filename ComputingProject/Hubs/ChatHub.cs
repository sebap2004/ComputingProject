using ComputingProject.Client.Services;
using Microsoft.AspNetCore.SignalR;

namespace ComputingProject.Hubs;


public class ChatHub : Hub<IChatHubClient>, IChatHubServer
{
    public bool IsConnected { get; }

    public async Task SendMessage(string sender, string content, bool systemMessage)
    {
        await Clients.All.ReceiveMessage(sender, content, systemMessage);
    }
}