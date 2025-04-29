using System.Security.Claims;
using ComputingProject.Client.Services;
using ComputingProject.Services;
using Microsoft.AspNetCore.SignalR;

namespace ComputingProject.Hubs;

/// <summary>
/// Represents a signalR hub that handles communication between clients and
/// manages state and interaction of a classroom environment.
/// This class is the main process of the entire program's operation. It handles all of the real-time functionality of the application.
/// It supports operations such as managing classroom
/// state, sending messages, handling questions, processing help requests, and
/// managing announcements.
/// </summary>
public class Classroom : Hub<IClassroomClient>, IClassroomServer
{
    /// <summary>
    /// Classroom service to be injected into the hub via dependency injection.
    /// </summary>
    private readonly ClassroomStateService ClassroomStateService;
    
    /// <summary>
    /// Constructor for the classroom hub.
    /// </summary>
    /// <param name="stateService">State service to be injected into the system.</param>
    public Classroom(ClassroomStateService stateService)
    {
        ClassroomStateService = stateService;
    }
    
    /// <summary>
    /// Called when a new client connects to the hub.
    /// Adds a client to their respective group based on their role.
    /// </summary>
    public override Task OnConnectedAsync()
    {
        Console.WriteLine("New Connection");
        if (Context.User != null)
        {
            string? Role = Context.User.FindFirst(ClaimTypes.Role)?.Value;
            Console.WriteLine("Role: " + Role);
            if (Role == "Student")
            {
                ClassroomStateService.AddStudent(Context.User.Identity.Name);
                Groups.AddToGroupAsync(Context.ConnectionId, "Student");
                Clients.All.GetStudentJoinedMessage(ClassroomStateService.ConnectedStudents);
                Console.WriteLine("sent student joined teacher");
            }
            if (Role  == "Teacher")
            {
                Groups.AddToGroupAsync(Context.ConnectionId, "Teacher");
                Console.WriteLine("added teacher to group");
            }
        }
        return Task.CompletedTask;
    }
    
    
    /// <summary>
    /// Called when a client disconnects from the hub.
    /// Dispatches relevant messages to relevant clients based on their role.
    /// </summary>
    /// <param name="exception">If the disconnection was caled by an exception.</param>
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine("New Disconnection");
        if (Context.User != null)
        {
            ClassroomStateService.RemoveStudent(Context.User.Identity.Name);
            ClassroomStateService.RemoveHelpRequest(Context.User.Identity.Name);
            if (Context.User.FindFirst(ClaimTypes.Role)?.Value == "Student")
            {
                Clients.All.GetStudentLeftMessage(ClassroomStateService.ConnectedStudents);
                Clients.All.GetActiveHelpRequests(ClassroomStateService.ActiveHelpRequests);
            }
        }
        return Task.CompletedTask;
    }
    
    /// <inheritdoc/>
    public async Task GetClassroomState()
    {
        await Clients.All.GetClassroomState(ClassroomStateService.classroomState);
    }

    /// <inheritdoc/>
    public async Task GetStudents()
    {
        Console.WriteLine("Get student list, count: " + ClassroomStateService.ConnectedStudents.Count);
        await Clients.All.GetStudents(ClassroomStateService.ConnectedStudents);
    }

    /// <inheritdoc/>
    public async Task GetActiveHelpRequests()
    {
        await Clients.All.GetActiveHelpRequests(ClassroomStateService.ActiveHelpRequests);
    }

    /// <inheritdoc/>
    public async Task GetActiveQuestions()
    {
        await Clients.All.GetActiveQuestions(ClassroomStateService.ActiveQuestions);
    }

    /// <inheritdoc/>
    public async Task GetCurrentTask()
    {
        await Clients.All.GetCurrentTask(ClassroomStateService.CurrentTask);
    }

    /// <inheritdoc/>
    public async Task GetAnnouncements()
    {
        await Clients.All.GetAnnouncements(ClassroomStateService.TeacherAnnouncements);
    }

    /// <inheritdoc/>
    public async Task SendTeacherQuestion(TeacherQuestion question)
    {
        Console.WriteLine("Sending teacher question to to connections");
        ClassroomStateService.AddQuestion(question);
        await Clients.All.GetActiveQuestions(ClassroomStateService.ActiveQuestions);
    }

    /// <inheritdoc/>
    public async Task AnswerTeacherQuestion(string studentID, string questionID, string answer)
    {
        ClassroomStateService.AddAnswerToQuestion(questionID, answer);
        await Clients.Group("Teacher").GetActiveQuestions(ClassroomStateService.ActiveQuestions);
        await Clients.User(studentID).GetActiveQuestions(ClassroomStateService.ActiveQuestions);
        Console.WriteLine("sending teacher question to to connections " + studentID);
    }

    /// <inheritdoc/>
    public async Task SendHelpRequest(string requestID)
    {
        ClassroomStateService.AddHelpRequest(requestID);
        await Clients.Group("Teacher").GetActiveHelpRequests(ClassroomStateService.ActiveHelpRequests);
    }

    /// <inheritdoc/>
    public async Task CancelHelpRequest(string requestID)
    {
        ClassroomStateService.RemoveHelpRequest(requestID);
        await Clients.Group("Teacher").GetActiveHelpRequests(ClassroomStateService.ActiveHelpRequests);
    }

    /// <inheritdoc/>
    public async Task AcknowledgeHelpRequest(string requestID)
    {
        await Clients.User(requestID).GetAcknowledgementForHelpRequest();
        await Clients.Group("Teacher").GetActiveHelpRequests(ClassroomStateService.ActiveHelpRequests);
        Console.WriteLine("Sending acknowledgement to " + requestID);
    }

    /// <inheritdoc/>
    public async Task ResolveHelpRequest(string requestID)
    {
        ClassroomStateService.RemoveHelpRequest(requestID);
        Console.WriteLine("Sending resolution to " + requestID);
        await Clients.User(requestID).GetResolutionForHelpRequest();        
        await Clients.Group("Teacher").GetActiveHelpRequests(ClassroomStateService.ActiveHelpRequests);
    }

    /// <inheritdoc/>
    public async Task ArchiveTeacherQuestion(string questionID)
    {
        ClassroomStateService.ToggleArchivedQuestion(questionID);
        await Clients.All.GetActiveQuestions(ClassroomStateService.ActiveQuestions);
    }

    /// <inheritdoc/>
    public async Task SetClassroomState(ClassroomState stateToChangeTo)
    {
        ClassroomStateService.classroomState = stateToChangeTo;
        await Clients.All.GetClassroomState(ClassroomStateService.classroomState);
    }

    /// <inheritdoc/>
    public async Task DeleteTeacherQuestion(string questionId)
    {
        ClassroomStateService.DeleteTeacherQuestion(questionId);
        await Clients.All.GetActiveQuestions(ClassroomStateService.ActiveQuestions);
    }

    /// <inheritdoc/>
    public async Task AddAnnouncement(TeacherAnnouncement announcement)
    {
        ClassroomStateService.AddTeacherAnnouncement(announcement);
        await Clients.All.GetAnnouncements(ClassroomStateService.TeacherAnnouncements);
    }

    /// <inheritdoc/>
    public async Task RemoveAnnouncement(string announcementId)
    {
        ClassroomStateService.RemoveTeacherAnnouncement(announcementId);
        await Clients.All.GetAnnouncements(ClassroomStateService.TeacherAnnouncements);
    }

    /// <inheritdoc/>
    public async Task ToggleHideAnnouncement(string announcementId)
    {
        ClassroomStateService.ToggleHideTeacherAnnouncement(announcementId);
        await Clients.All.GetAnnouncements(ClassroomStateService.TeacherAnnouncements);
    }

    /// <inheritdoc/>
    public async Task SetCurrentTask(string task)
    {
        ClassroomStateService.SetCurrentTask(task);
        await Clients.All.GetCurrentTask(ClassroomStateService.CurrentTask);
    }
}