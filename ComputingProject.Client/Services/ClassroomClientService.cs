using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace ComputingProject.Client.Services;

public class ClassroomClientService : IClassroomClient, IClassroomService
{
    private readonly HubConnection _hubConnection;
    public bool IsConnected() => _hubConnection.State == HubConnectionState.Connected;

    public event Action<string, string, bool>? OnMessageReceived;
    public event Action<ClassroomState>? OnJoinGetState;
    public event Action<ClassroomState>? GetState;
    public event Action<List<String>>? OnStudentJoinedMessage;
    public event Action<List<String>>? OnStudentLeftMessage;
    public event Action<List<string>>? OnReceiveStudentList;


    public ClassroomClientService(HubConnection hubConnection)
    {
        _hubConnection = hubConnection;
        _hubConnection.On<string, string, bool>("ReceiveMessage", ReceiveMessage);
        _hubConnection.On<ClassroomState>("GetClassroomState", GetClassroomState);
        _hubConnection.On<List<String>>("SendStudentJoinedMessage", SendStudentJoinedMessage);
        _hubConnection.On<List<String>>("SendStudentLeftMessage", SendStudentLeftMessage);
        _hubConnection.On<List<String>>("GetStudents", GetStudents);
    }

    public async Task StartAsync()
    {
        try
        {
            Console.WriteLine("Starting classroom hub connection...");
            await _hubConnection.StartAsync();
            Console.WriteLine("classroom hub connection started successfully. Current state: " + _hubConnection.State);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting classroom hub connection: {ex.Message}");
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
        GetState?.Invoke(classroomState);
        Console.WriteLine("Invoked message");
        return Task.CompletedTask;
    }

    public Task GetStudents(List<string> students)
    {
        Console.WriteLine("Received Students");
        OnReceiveStudentList?.Invoke(students);
        return Task.CompletedTask;
    }

    public Task SendStudentJoinedMessage(List<String> Users)
    {
        OnStudentJoinedMessage?.Invoke(Users);
        return Task.CompletedTask;
    }

    public Task SendStudentLeftMessage(List<String> Users)
    {
        OnStudentLeftMessage?.Invoke(Users);
        return Task.CompletedTask;
    }
}

public enum ClassroomState
{
    Lecture,
    Seminar
}
