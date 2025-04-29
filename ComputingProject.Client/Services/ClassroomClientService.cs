using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace ComputingProject.Client.Services;

/// <summary>
/// Client service to handle classroom-related actions and events on the client side.
/// </summary>
public class ClassroomClientService : IClassroomClient, IClassroomService
{
    /// <summary>
    /// SignalR hub connection object. This handles the connection to the classroom.
    /// </summary>
    private readonly HubConnection _hubConnection;
    
    /// <summary>
    /// Helper expression bodied method to easily determine if the user is connected to the classroom or not
    /// </summary>
    /// <returns>Boolean representing connection status</returns>
    public bool IsConnected() => _hubConnection.State == HubConnectionState.Connected;
    
    /// <inheritdoc/>
    public event Action<ClassroomState>? GetState;
    /// <inheritdoc/>
    public event Action<List<String>>? OnStudentJoinedMessage;
    /// <inheritdoc/>
    public event Action<List<String>>? OnStudentLeftMessage;
    /// <inheritdoc/>
    public event Action<List<string>>? OnReceiveStudentList;
    /// <inheritdoc/>
    public event Action<List<string>>? OnReceiveActiveHelpRequests;
    /// <inheritdoc/>
    public event Action<List<TeacherQuestion>>? OnReceiveActiveQuestions;
    /// <inheritdoc/>
    public event Action<List<TeacherAnnouncement>>? OnReceiveAnnouncements;
    /// <inheritdoc/>
    public event Action<string>? OnReceiveCurrentTask;
    /// <inheritdoc/>
    public event Action<string>? OnArchiveTeacherQuestion;
    /// <inheritdoc/>
    public event Action? OnAcknowledgeHelpRequest;
    /// <inheritdoc/>
    public event Action? OnResolveHelpRequest;
    /// <inheritdoc/>
    public event Action<string>? OnAnsweredTeacherQuestion;


    /// <summary>
    /// Constructor. Injects the hub connection into the class, and subscribes to all of the necessary hub collection events.
    /// </summary>
    /// <param name="hubConnection"></param>
    public ClassroomClientService(HubConnection hubConnection)
    {
        _hubConnection = hubConnection;
        _hubConnection.On<ClassroomState>("GetClassroomState", GetClassroomState);
        _hubConnection.On<List<string>>("GetStudentJoinedMessage", GetStudentJoinedMessage);
        _hubConnection.On<List<string>>("GetStudentLeftMessage", GetStudentLeftMessage);
        _hubConnection.On<List<string>>("GetStudents", GetStudents);
        _hubConnection.On<List<string>>("GetActiveHelpRequests", GetActiveHelpRequests);
        _hubConnection.On<List<TeacherQuestion>>("GetActiveQuestions", GetActiveQuestions);
        _hubConnection.On<string>("ArchiveTeacherQuestion", ArchiveTeacherMethods);
        _hubConnection.On("GetAcknowledgementForHelpRequest", AcknowledgeHelpRequest);
        _hubConnection.On("GetResolutionForHelpRequest", ResolveHelpRequest);
        _hubConnection.On<string, string>("AnswerTeacherQuestion", AnswerTeacherQuestion);
        _hubConnection.On<List<TeacherAnnouncement>>("GetAnnouncements", GetAnnouncements);
        _hubConnection.On<string>("GetCurrentTask", GetCurrentTask);
    }
    
    /// <inheritdoc/>
    public Task AnswerTeacherQuestion(string questionID, string answer)
    {
        OnAnsweredTeacherQuestion?.Invoke(questionID);
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Event function received when acknowledging help request.
    /// </summary>
    private Task AcknowledgeHelpRequest()
    {
        Console.WriteLine("Received acknowledgement");
        OnAcknowledgeHelpRequest?.Invoke();
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Event function received when resolving a help request
    /// </summary>
    private Task ResolveHelpRequest()
    {
        Console.WriteLine("Received help");
        OnResolveHelpRequest?.Invoke();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Event function received when archiving a teacher's question
    /// </summary>
    private Task ArchiveTeacherMethods(string obj)
    {
        OnArchiveTeacherQuestion?.Invoke(obj);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public async Task StopAsync()
    {
        await _hubConnection.StopAsync();
    }
    
    /// <inheritdoc/>
    public Task GetClassroomState(ClassroomState classroomState)
    {
        GetState?.Invoke(classroomState);
        Console.WriteLine("Invoked message");
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task GetStudents(List<string> students)
    {
        Console.WriteLine("Received Students");
        OnReceiveStudentList?.Invoke(students);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task GetActiveHelpRequests(List<string> requests)
    {
        Console.WriteLine("Received Active Help Requests");
        OnReceiveActiveHelpRequests?.Invoke(requests);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task GetActiveQuestions(List<TeacherQuestion> students)
    {
        Console.WriteLine("GetActiveQuestions");
        OnReceiveActiveQuestions?.Invoke(students);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task GetAnnouncements(List<TeacherAnnouncement> announcements)
    {
        OnReceiveAnnouncements?.Invoke(announcements);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task GetCurrentTask(string task)
    {
        OnReceiveCurrentTask?.Invoke(task);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task GetStudentJoinedMessage(List<String> Users)
    {
        OnStudentJoinedMessage?.Invoke(Users);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task GetStudentLeftMessage(List<String> Users)
    {
        OnStudentLeftMessage?.Invoke(Users);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task GetAcknowledgementForHelpRequest()
    {
        OnAcknowledgeHelpRequest?.Invoke();
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task GetResolutionForHelpRequest()
    {
        OnResolveHelpRequest?.Invoke();
        return Task.CompletedTask;
    }
}

/// <summary>
/// Enum representing the classroom's state.
/// </summary>
public enum ClassroomState
{
    Lecture,
    Workshop
}
