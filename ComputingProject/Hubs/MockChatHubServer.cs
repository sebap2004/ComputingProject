using ComputingProject.Client.Services;

namespace ComputingProject.Hubs;

public class MockChatHubServer : IChatHubServer
{
    public Task SendMessage(string sender, string content, bool systemMessage) => Task.CompletedTask;
}