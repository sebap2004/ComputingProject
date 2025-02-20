using Microsoft.AspNetCore.SignalR.Client;

namespace ComputingProject.Client.Services;

public class ClassroomServerProxy : IClassroomServer
{
    private readonly HubConnection _hubConnection;
    
    public ClassroomServerProxy(HubConnection hubConnection)
    {
        Console.WriteLine("Added ChatHubClientService");
        _hubConnection = hubConnection;
    }

    public async Task SendMessage(string sender, string content, bool systemMessage)
    {
        await _hubConnection.InvokeAsync("SendMessage", sender, content, systemMessage);
    }
}
