using Microsoft.AspNetCore.SignalR.Client;

namespace ComputingProject.Client.Services;

public class ClassroomServerProxy : IClassroomServer
{
    private readonly HubConnection _hubConnection;
    
    public ClassroomServerProxy(HubConnection hubConnection)
    {
        Console.WriteLine("Added ChatHubClientService");
        _hubConnection = hubConnection;
    }

    public async Task SendMessage(string sender, string content, bool systemMessage)
    {
        await _hubConnection.InvokeAsync("SendMessage", sender, content, systemMessage);
    }

    public async Task SendMessageToTeacher(string sender, string content, bool systemMessage)
    {
        await _hubConnection.InvokeAsync("SendMessageToTeacher", sender, content, systemMessage);
    }

    public async Task GetClassroomState(string userID)
    {
        await _hubConnection.InvokeAsync("GetClassroomState", userID);
        Console.WriteLine("Invoked GetClassroomState");
    }

    public async Task GetStudents()
    {
        await _hubConnection.InvokeAsync("GetStudents");
        Console.WriteLine("Invoked GetStudents");
    }

    public async Task GetActiveHelpRequests()
    {
        await _hubConnection.InvokeAsync("GetActiveHelpRequests");
    }

    public async Task GetActiveQuestions()
    {
        await _hubConnection.InvokeAsync("GetActiveQuestions");
    }

    public async Task GetCurrentTask()
    {
        await _hubConnection.InvokeAsync("GetCurrentTask");
    }

    public async Task GetAnnouncements()
    {
        await _hubConnection.InvokeAsync("GetAnnouncements");
    }

    public async Task SendTeacherQuestion(TeacherQuestion question)
    {
        Console.WriteLine("Sending tq message to hub: " + question.QuestionText + " - " + question.Archived);
        await _hubConnection.InvokeAsync("SendTeacherQuestion", question);
    }

    public async Task AnswerTeacherQuestion(string studentID, string questionID, string answer)
    {
        await _hubConnection.InvokeAsync("AnswerTeacherQuestion", studentID, questionID, answer);
    }

    public async Task SendHelpRequest(string requestID)
    {
        await _hubConnection.InvokeAsync("SendHelpRequest", requestID);
    }

    public async Task CancelHelpRequest(string requestID)
    {
        await _hubConnection.InvokeAsync("CancelHelpRequest", requestID);
    }

    public async Task AcknowledgeHelpRequest(string requestID)
    {
        await _hubConnection.InvokeAsync("AcknowledgeHelpRequest", requestID);
    }

    public async Task ResolveHelpRequest(string requestID)
    {
        await _hubConnection.InvokeAsync("ResolveHelpRequest", requestID);
    }

    public async Task ArchiveTeacherQuestion(string questionID)
    {
        await _hubConnection.InvokeAsync("ArchiveTeacherQuestion", questionID);
    }

    public async Task SetClassroomState(ClassroomState stateToChangeTo)
    {
        await _hubConnection.InvokeAsync("SetClassroomState", stateToChangeTo);
    }

    public async Task DeleteTeacherQuestion(string questionId)
    {
        await _hubConnection.InvokeAsync("DeleteTeacherQuestion", questionId);
    }

    public async Task AddAnnouncement(TeacherAnnouncement announcement)
    {
        await _hubConnection.InvokeAsync("AddAnnouncement", announcement);
    }

    public async Task RemoveAnnouncement(string announcementId)
    {
        await _hubConnection.InvokeAsync("RemoveAnnouncement", announcementId);
    }

    public async Task ToggleHideAnnouncement(string announcementId)
    {
        await _hubConnection.InvokeAsync("ToggleHideAnnouncement", announcementId);
    }

    public async Task SetCurrentTask(string task)
    {
        await _hubConnection.InvokeAsync("SetCurrentTask", task);
    }
}
