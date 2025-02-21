using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace ComputingProject.Client.Services;

public class ClassroomClientService : IClassroomClient, IClassroomService
{
    private readonly HubConnection _hubConnection;
    public bool IsConnected() => _hubConnection.State == HubConnectionState.Connected;

    public event Action<string, string, bool>? OnMessageReceived;
    public event Action<ClassroomState>? OnJoinGetState;

    

    public ClassroomClientService(HubConnection hubConnection)
    {
        Console.WriteLine("Added ChatHubClientService");
        _hubConnection = hubConnection;
        // Receiving Chat Message
        _hubConnection.On<string, string, bool>("ReceiveMessage", (user, message, systemMessage) =>
        {
            OnMessageReceived?.Invoke(user, message, systemMessage);
        });
        _hubConnection.On<ClassroomState>("GetClassroomState", (state) =>
        {
            OnJoinGetState?.Invoke(state);
        });
    }

    public async Task StartAsync()
    {
        try
        {
            Console.WriteLine("Starting chat hub connection...");
            await _hubConnection.StartAsync();
            Console.WriteLine("Chat hub connection started successfully. Current state: " + _hubConnection.State);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting chat hub connection: {ex.Message}");
        }
    }

    public async Task StopAsync()
    {
        await _hubConnection.StopAsync();
    }

    public Task ReceiveMessage(string sender, string content, bool systemMessage)
    {
        OnMessageReceived?.Invoke(sender, content, systemMessage);
        return Task.CompletedTask;
    }

    public Task GetClassroomState(ClassroomState classroomState)
    {
        OnJoinGetState?.Invoke(classroomState);
        Console.WriteLine("Invoked message");
        return Task.CompletedTask;
    }
}

public enum ClassroomState
{
    Lecture,
    Seminar
}
