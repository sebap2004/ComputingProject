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

    /// <inheritdoc/>
    public async Task GetClassroomState()
    {
        await _hubConnection.InvokeAsync("GetClassroomState");
        Console.WriteLine("Invoked GetClassroomState");
    }

    /// <inheritdoc/>
    public async Task GetStudents()
    {
        await _hubConnection.InvokeAsync("GetStudents");
        Console.WriteLine("Invoked GetStudents");
    }

    /// <inheritdoc/>
    public async Task GetActiveHelpRequests()
    {
        await _hubConnection.InvokeAsync("GetActiveHelpRequests");
    }

    /// <inheritdoc/>
    public async Task GetActiveQuestions()
    {
        await _hubConnection.InvokeAsync("GetActiveQuestions");
    }

    /// <inheritdoc/>
    public async Task GetCurrentTask()
    {
        await _hubConnection.InvokeAsync("GetCurrentTask");
    }

    /// <inheritdoc/>
    public async Task GetAnnouncements()
    {
        await _hubConnection.InvokeAsync("GetAnnouncements");
    }

    /// <inheritdoc/>
    public async Task SendTeacherQuestion(TeacherQuestion question)
    {
        Console.WriteLine("Sending tq message to hub: " + question.QuestionText + " - " + question.Archived);
        await _hubConnection.InvokeAsync("SendTeacherQuestion", question);
    }

    /// <inheritdoc/>
    public async Task AnswerTeacherQuestion(string studentID, string questionID, string answer)
    {
        await _hubConnection.InvokeAsync("AnswerTeacherQuestion", studentID, questionID, answer);
    }

    /// <inheritdoc/>
    public async Task SendHelpRequest(string requestID)
    {
        await _hubConnection.InvokeAsync("SendHelpRequest", requestID);
    }

    /// <inheritdoc/>
    public async Task CancelHelpRequest(string requestID)
    {
        await _hubConnection.InvokeAsync("CancelHelpRequest", requestID);
    }

    /// <inheritdoc/>
    public async Task AcknowledgeHelpRequest(string requestID)
    {
        await _hubConnection.InvokeAsync("AcknowledgeHelpRequest", requestID);
    }

    /// <inheritdoc/>
    public async Task ResolveHelpRequest(string requestID)
    {
        await _hubConnection.InvokeAsync("ResolveHelpRequest", requestID);
    }

    /// <inheritdoc/>
    public async Task ArchiveTeacherQuestion(string questionID)
    {
        await _hubConnection.InvokeAsync("ArchiveTeacherQuestion", questionID);
    }

    /// <inheritdoc/>
    public async Task SetClassroomState(ClassroomState stateToChangeTo)
    {
        await _hubConnection.InvokeAsync("SetClassroomState", stateToChangeTo);
    }

    /// <inheritdoc/>
    public async Task DeleteTeacherQuestion(string questionId)
    {
        await _hubConnection.InvokeAsync("DeleteTeacherQuestion", questionId);
    }

    /// <inheritdoc/>
    public async Task AddAnnouncement(TeacherAnnouncement announcement)
    {
        await _hubConnection.InvokeAsync("AddAnnouncement", announcement);
    }

    /// <inheritdoc/>
    public async Task RemoveAnnouncement(string announcementId)
    {
        await _hubConnection.InvokeAsync("RemoveAnnouncement", announcementId);
    }

    /// <inheritdoc/>
    public async Task ToggleHideAnnouncement(string announcementId)
    {
        await _hubConnection.InvokeAsync("ToggleHideAnnouncement", announcementId);
    }

    /// <inheritdoc/>
    public async Task SetCurrentTask(string task)
    {
        await _hubConnection.InvokeAsync("SetCurrentTask", task);
    }
}
