using ComputingProject.Client.Services;

namespace ComputingProject.Hubs;

public class MockChatHubClient : IChatHubClient
{
    public bool IsConnected() => false;

    public event Action<string, string, bool> OnMessageReceived; // No-op event
    public Task StartAsync() => Task.CompletedTask;
    public Task StopAsync() => Task.CompletedTask;
    public Task ReceiveMessage(string sender, string content, bool systemMessage) => Task.CompletedTask;
}