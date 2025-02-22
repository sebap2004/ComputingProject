using System.Security.Claims;
using ComputingProject.Client.Services;

namespace ComputingProject.Hubs;

public class MockClassroomService : IClassroomService
{
    public event Action<string, string, bool>? OnMessageReceived;
    public event Action<ClassroomState>? OnJoinGetState;
    public event Action<ClassroomState>? GetState;
    public event Action<List<String>>? OnStudentJoinedMessage;
    public event Action<List<String>>? OnStudentLeftMessage;
    public event Action<List<string>>? OnReceiveStudentList;
    public Task StartAsync() => Task.CompletedTask;
    public Task StopAsync() => Task.CompletedTask;
    public bool IsConnected() => false;
}