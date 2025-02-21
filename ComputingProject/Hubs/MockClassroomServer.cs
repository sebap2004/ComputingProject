using ComputingProject.Client.Services;

namespace ComputingProject.Hubs;

public class MockClassroomServer : IClassroomServer
{
    public Task SendMessage(string sender, string content, bool systemMessage) => Task.CompletedTask;
    public Task GetClassroomState() => Task.CompletedTask;
    public Task SetClassroomState(ClassroomState stateToChangeTo)
    {
        return Task.CompletedTask;
    }
}