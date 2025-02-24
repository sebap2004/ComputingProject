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

    public Task GetActiveHelpRequests()
    {
        return Task.CompletedTask;
        
    }

    public Task GetActiveQuestions()
    {
        return Task.CompletedTask;
        
    }

    public Task SendTeacherQuestion(TeacherQuestion question)
    {
        return Task.CompletedTask;
    }

    public Task AnswerTeacherQuestion(string studentID, string questionID, string answer)
    {
        return Task.CompletedTask;
    }

    public Task AnswerTeacherQuestion(string questionID, string answer)
    {
        return Task.CompletedTask;
    }

    public Task SendHelpRequest(string requestID)
    {
        return Task.CompletedTask;
    }

    public Task CancelHelpRequest(string requestID)
    {
        return Task.CompletedTask;
    }

    public Task AcknowledgeHelpRequest(string requestID)
    {
        return Task.CompletedTask;
    }

    public Task ResolveHelpRequest(string requestID)
    {
        return Task.CompletedTask;
    }

    public Task ArchiveTeacherQuestion(string questionID)
    {
        return Task.CompletedTask;
    }

    public Task SetClassroomState(ClassroomState stateToChangeTo)
    {
        return Task.CompletedTask;
    }
}