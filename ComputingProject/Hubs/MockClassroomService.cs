using ComputingProject.Client.Services;

namespace ComputingProject.Hubs;

public class MockClassroomService : IClassroomService
{
    public event Action<string, string, bool>? OnMessageReceived;
    public event Action<ClassroomState>? OnJoinGetState;
    public Task StartAsync() => Task.CompletedTask;
    public Task StopAsync() => Task.CompletedTask;
    public bool IsConnected() => false;
}