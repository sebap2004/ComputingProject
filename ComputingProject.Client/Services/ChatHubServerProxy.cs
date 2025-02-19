using Microsoft.AspNetCore.SignalR.Client;

namespace ComputingProject.Client.Services;

public class ChatHubServerProxy : IChatHubServer
{
    private readonly HubConnection _hubConnection;

    public bool IsConnected => _hubConnection.State == HubConnectionState.Connected;
    
    public ChatHubServerProxy(HubConnection hubConnection)
    {
        Console.WriteLine("Added ChatHubClientService");
        _hubConnection = hubConnection;
    }

    public async Task SendMessage(string sender, string content, bool systemMessage)
    {
        await _hubConnection.InvokeAsync("SendMessage", sender, content, systemMessage);
    }
}
