using ComputingProject.Client.Services;

namespace ComputingProject.Hubs;

public class MockChatService : IChatService
{
    public event Action<string, string, bool>? OnMessageReceived;
    public Task StartAsync() => Task.CompletedTask;
    public Task StopAsync() => Task.CompletedTask;
    public bool IsConnected() => false;
}