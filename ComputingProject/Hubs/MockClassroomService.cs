using System.Security.Claims;
using ComputingProject.Client.Services;

namespace ComputingProject.Hubs;

public class MockClassroomService : IClassroomService
{
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
    public Task StartAsync() => Task.CompletedTask;
    public Task StopAsync() => Task.CompletedTask;
    public bool IsConnected() => false;
}