using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ComputingProject.Client.Services;

namespace ComputingProject.Hubs;


/// <summary>
/// Plcaeholder classroom client that does nothing. It is used to provide a default value for the client while it refreshes the page.
/// This has been created so the website doesn't throw an error when refreshing the page.
/// </summary>
public class PlaceholderClassroomClient : IClassroomClient
{
    public Task GetClassroomState(ClassroomState classroomState)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task GetStudents(List<string> students)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task GetActiveHelpRequests(List<string> students)
    {
        return Task.CompletedTask;
        
    }

    /// <inheritdoc/>
    public Task GetActiveQuestions(List<TeacherQuestion> questions)
    {
        return Task.CompletedTask;
        
    }

    /// <inheritdoc/>
    public Task GetAnnouncements(List<TeacherAnnouncement> announcements)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task GetCurrentTask(string task)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task GetStudentJoinedMessage(List<string> Users)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task GetStudentLeftMessage(List<string> Users)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task GetAcknowledgementForHelpRequest()
    {
        return Task.CompletedTask;
        
    }

    /// <inheritdoc/>
    public Task GetResolutionForHelpRequest()
    {
        return Task.CompletedTask;
        
    }

    /// <inheritdoc/>
    public Task AnswerTeacherQuestion(string questionID, string answer)
    {
        return Task.CompletedTask;
        
    }
}