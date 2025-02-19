using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace ComputingProject.Client.Services;

public class ChatHubClientService : IChatHubClient
{
    private readonly HubConnection _hubConnection;

    public event Action<string, string, bool>? OnMessageReceived;

    public ChatHubClientService(NavigationManager navigation)
    {
        Console.WriteLine("Added ChatHubClientService");
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(navigation.ToAbsoluteUri("/chatHub"))
            .Build();
        _hubConnection.On<string, string, bool>("ReceiveMessage", (user, message, systemMessage) =>
        {
            OnMessageReceived?.Invoke(user, message, systemMessage);
        });
    }

    public async Task StartAsync()
    {
        Console.WriteLine("Started chat hub connection");
        await _hubConnection.StartAsync();
    }

    public async Task StopAsync()
    {
        await _hubConnection.StopAsync();
    }

    public async Task ReceiveMessage(string sender, string content, bool systemMessage)
    {
        OnMessageReceived?.Invoke(sender, content, systemMessage);
    }
}
