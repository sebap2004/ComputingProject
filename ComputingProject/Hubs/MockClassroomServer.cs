using ComputingProject.Client.Services;

namespace ComputingProject.Hubs;

public class MockClassroomServer : IClassroomServer
{
    public Task SendMessage(string sender, string content, bool systemMessage) => Task.CompletedTask;
    public Task SendMessageToTeacher(string sender, string content, bool systemMessage)
    {
        return Task.CompletedTask;
    }

    public Task GetClassroomState(string user) => Task.CompletedTask;
    public Task GetStudents()
    {
        return Task.CompletedTask;
    }

    public Task SetClassroomState(ClassroomState stateToChangeTo)
    {
        return Task.CompletedTask;
    }
}