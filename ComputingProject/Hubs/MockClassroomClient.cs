using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ComputingProject.Client.Services;

namespace ComputingProject.Hubs;

public class MockClassroomClient : IClassroomClient
{
    public bool IsConnected() => false;

    public event Action<string, string, bool> OnMessageReceived; // No-op event
    public Task StartAsync() => Task.CompletedTask;
    public Task StopAsync() => Task.CompletedTask;
    public Task GetMessage(string sender, string content, bool systemMessage) => Task.CompletedTask;
    public Task GetClassroomState(ClassroomState classroomState)
    {
        return Task.CompletedTask;
    }

    public Task GetStudents(List<string> students)
    {
        return Task.CompletedTask;
    }

    public Task GetActiveHelpRequests(List<string> students)
    {
        return Task.CompletedTask;
        
    }

    public Task GetActiveQuestions(List<TeacherQuestion> questions)
    {
        return Task.CompletedTask;
        
    }

    public Task GetAnnouncements(List<TeacherAnnouncement> announcements)
    {
        return Task.CompletedTask;
    }

    public Task GetCurrentTask(string task)
    {
        return Task.CompletedTask;
    }

    public Task GetStudentJoinedMessage(List<string> Users)
    {
        return Task.CompletedTask;
    }

    public Task GetStudentLeftMessage(List<string> Users)
    {
        return Task.CompletedTask;
    }

    public Task GetAcknowledgementForHelpRequest()
    {
        return Task.CompletedTask;
        
    }

    public Task GetResolutionForHelpRequest()
    {
        return Task.CompletedTask;
        
    }

    public Task AnswerTeacherQuestion(string questionID, string answer)
    {
        return Task.CompletedTask;
        
    }

    public Task SendStudentJoinedMessage(List<ClaimsPrincipal> Users)
    {
        return Task.CompletedTask;
    }

    public Task SendStudentLeftMessage(List<ClaimsPrincipal> Users)
    {
        return Task.CompletedTask;
    }
}