using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace ComputingProject.Client.Services;

public class ClassroomClientService : IClassroomClient, IClassroomService
{
    private readonly HubConnection _hubConnection;
    public bool IsConnected() => _hubConnection.State == HubConnectionState.Connected;

    public event Action<string, string, bool>? OnMessageReceived;
    public event Action<ClassroomState>? OnJoinGetState;
    public event Action<ClassroomState>? GetState;
    public event Action<List<String>>? OnStudentJoinedMessage;
    public event Action<List<String>>? OnStudentLeftMessage;
    public event Action<List<string>>? OnReceiveStudentList;
    public event Action<List<string>>? OnReceiveActiveHelpRequests;
    public event Action<List<TeacherQuestion>>? OnReceiveActiveQuestions;
    public event Action<List<TeacherAnnouncement>>? OnReceiveAnnouncements;
    public event Action<string>? OnReceiveCurrentTask;
    public event Action<string>? OnArchiveTeacherQuestion;
    public event Action? OnAcknowledgeHelpRequest;
    public event Action? OnResolveHelpRequest;
    public event Action<string>? OnAnsweredTeacherQuestion;


    public ClassroomClientService(HubConnection hubConnection)
    {
        _hubConnection = hubConnection;
        _hubConnection.On<string, string, bool>("ReceiveMessage", GetMessage);
        _hubConnection.On<ClassroomState>("GetClassroomState", GetClassroomState);
        _hubConnection.On<List<String>>("SendStudentJoinedMessage", GetStudentJoinedMessage);
        _hubConnection.On<List<String>>("SendStudentLeftMessage", GetStudentLeftMessage);
        _hubConnection.On<List<String>>("GetStudents", GetStudents);
        _hubConnection.On<List<String>>("GetActiveHelpRequests", GetActiveHelpRequests);
        _hubConnection.On<List<TeacherQuestion>>("GetActiveQuestions", GetActiveQuestions);
        _hubConnection.On<string>("ArchiveTeacherQuestion", ArchiveTeacherMethods);
        _hubConnection.On("ReceiveAcknowledgementForHelpRequest", AcknowledgeHelpRequest);
        _hubConnection.On("ReceiveResolutionForHelpRequest", ResolveHelpRequest);
        _hubConnection.On<string, string>("AnswerTeacherQuestion", AnswerTeacherQuestion);
        _hubConnection.On<List<TeacherAnnouncement>>("GetAnnouncements", GetAnnouncements);
        _hubConnection.On<string>("GetCurrentTask", GetCurrentTask);
    }

    public Task AnswerTeacherQuestion(string questionID, string answer)
    {
        OnAnsweredTeacherQuestion?.Invoke(questionID);
        return Task.CompletedTask;
    }

    private Task AcknowledgeHelpRequest()
    {
        Console.WriteLine("Received acknowledgement");
        OnAcknowledgeHelpRequest?.Invoke();
        return Task.CompletedTask;
    }

    private Task ResolveHelpRequest()
    {
        Console.WriteLine("Received help");
        OnResolveHelpRequest?.Invoke();
        return Task.CompletedTask;
    }

    private Task ArchiveTeacherMethods(string obj)
    {
        OnArchiveTeacherQuestion?.Invoke(obj);
        return Task.CompletedTask;
    }

    public async Task StartAsync()
    {
        try
        {
            Console.WriteLine("Starting classroom hub connection...");
            await _hubConnection.StartAsync();
            Console.WriteLine("classroom hub connection started successfully. Current state: " + _hubConnection.State);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting classroom hub connection: {ex.Message}");
        }
    }

    public async Task StopAsync()
    {
        await _hubConnection.StopAsync();
    }

    public Task GetMessage(string sender, string content, bool systemMessage)
    {
        OnMessageReceived?.Invoke(sender, content, systemMessage);
        return Task.CompletedTask;
    }

    public Task GetClassroomState(ClassroomState classroomState)
    {
        GetState?.Invoke(classroomState);
        Console.WriteLine("Invoked message");
        return Task.CompletedTask;
    }

    public Task GetStudents(List<string> students)
    {
        Console.WriteLine("Received Students");
        OnReceiveStudentList?.Invoke(students);
        return Task.CompletedTask;
    }

    public Task GetActiveHelpRequests(List<string> requests)
    {
        Console.WriteLine("Received Active Help Requests");
        OnReceiveActiveHelpRequests?.Invoke(requests);
        return Task.CompletedTask;
    }

    public Task GetActiveQuestions(List<TeacherQuestion> students)
    {
        Console.WriteLine("GetActiveQuestions");
        OnReceiveActiveQuestions?.Invoke(students);
        return Task.CompletedTask;
    }

    public Task GetAnnouncements(List<TeacherAnnouncement> announcements)
    {
        OnReceiveAnnouncements?.Invoke(announcements);
        return Task.CompletedTask;
    }

    public Task GetCurrentTask(string task)
    {
        OnReceiveCurrentTask?.Invoke(task);
        return Task.CompletedTask;
    }

    public Task GetStudentJoinedMessage(List<String> Users)
    {
        OnStudentJoinedMessage?.Invoke(Users);
        return Task.CompletedTask;
    }

    public Task GetStudentLeftMessage(List<String> Users)
    {
        OnStudentLeftMessage?.Invoke(Users);
        return Task.CompletedTask;
    }

    public Task GetAcknowledgementForHelpRequest()
    {
        OnAcknowledgeHelpRequest?.Invoke();
        return Task.CompletedTask;
    }

    public Task GetResolutionForHelpRequest()
    {
        OnResolveHelpRequest?.Invoke();
        return Task.CompletedTask;
    }
}

public enum ClassroomState
{
    Lecture,
    Workshop
}
