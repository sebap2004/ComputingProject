using System.Security.Claims;
using ComputingProject.Client.Services;

namespace ComputingProject.Hubs;

/// <summary>
/// Placeholder classroom service that does nothing. It is used to provide a default value for the client while it refreshes the page.
/// This has been created so the website doesn't throw an error when refreshing the page.
/// </summary>
public class PlaceholderClassroomService : IClassroomService
{
    
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
    /// <inheritdoc/>
    public Task StartAsync() => Task.CompletedTask;
    /// <inheritdoc/>
    public Task StopAsync() => Task.CompletedTask;
    /// <inheritdoc/>
    public bool IsConnected() => false;
    /// <inheritdoc/>
}