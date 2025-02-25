using System.Security.Claims;
using ComputingProject.Client.Services;
using ComputingProject.Services;
using Microsoft.AspNetCore.SignalR;

namespace ComputingProject.Hubs;


public class Classroom : Hub<IClassroomClient>, IClassroomServer
{
    private readonly ClassroomStateService ClassroomStateService;
    public Classroom(ClassroomStateService stateService)
    {
        ClassroomStateService = stateService;
    }
    
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
                Clients.Group("Teacher").SendStudentJoinedMessage(ClassroomStateService.ConnectedStudents);
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
    
    

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine("New Disconnection");
        if (Context.User != null)
        {
            ClassroomStateService.RemoveStudent(Context.User.Identity.Name);
            if (Context.User.FindFirst(ClaimTypes.Role)?.Value == "Student")
            {
                Clients.Group("Teacher").SendStudentLeftMessage(ClassroomStateService.ConnectedStudents);
            }
        }
        return Task.CompletedTask;
    }

    public async Task SendMessage(string sender, string content, bool systemMessage)
    {
        await Clients.All.ReceiveMessage(sender, content, systemMessage);
    }
    
    public async Task SendMessageToTeacher(string sender, string content, bool systemMessage)
    {
        await Clients.User("Teacher").ReceiveMessage(sender + " - TEACHER ONLY", content, systemMessage);
        await Clients.User(sender).ReceiveMessage(sender, content, systemMessage);
    }

    public async Task GetClassroomState(string UserName)
    {
        Console.WriteLine("Sent state to clients");
        await Clients.User(UserName).GetClassroomState(ClassroomStateService.classroomState);
    }

    public async Task GetStudents()
    {
        Console.WriteLine("Get student list, count: " + ClassroomStateService.ConnectedStudents.Count);
        await Clients.Group("Teacher").GetStudents(ClassroomStateService.ConnectedStudents);
    }

    public async Task GetActiveHelpRequests()
    {
        await Clients.Group("Teacher").GetActiveHelpRequests(ClassroomStateService.ActiveHelpRequests);
    }

    public async Task GetActiveQuestions()
    {
        await Clients.All.GetActiveQuestions(ClassroomStateService.ActiveQuestions);
    }

    public async Task SendTeacherQuestion(TeacherQuestion question)
    {
        Console.WriteLine("Sending teacher question to to connections");
        ClassroomStateService.AddQuestion(question);
        await Clients.All.GetActiveQuestions(ClassroomStateService.ActiveQuestions);
    }

    public async Task AnswerTeacherQuestion(string studentID, string questionID, string answer)
    {
        ClassroomStateService.AddAnswerToQuestion(questionID, answer);
        await Clients.Group("Teacher").GetActiveQuestions(ClassroomStateService.ActiveQuestions);
        await Clients.User(studentID).GetActiveQuestions(ClassroomStateService.ActiveQuestions);
        Console.WriteLine("sending teacher question to to connections " + studentID);
    }

    public async Task SendHelpRequest(string requestID)
    {
        ClassroomStateService.AddHelpRequest(requestID);
        await Clients.Group("Teacher").GetActiveHelpRequests(ClassroomStateService.ActiveHelpRequests);
    }

    public async Task CancelHelpRequest(string requestID)
    {
        ClassroomStateService.RemoveHelpRequest(requestID);
        await Clients.Group("Teacher").GetActiveHelpRequests(ClassroomStateService.ActiveHelpRequests);
    }

    public async Task AcknowledgeHelpRequest(string requestID)
    {
        await Clients.Group("Student").ReceiveAcknowledgementForHelpRequest();
        await Clients.Group("Teacher").GetActiveHelpRequests(ClassroomStateService.ActiveHelpRequests);
        Console.WriteLine("Sending acknowledgement to " + requestID);
    }

    public async Task ResolveHelpRequest(string requestID)
    {
        ClassroomStateService.RemoveHelpRequest(requestID);
        Console.WriteLine("Sending resolution to " + requestID);
        await Clients.Group("Student").ReceiveResolutionForHelpRequest();        
        await Clients.Group("Teacher").GetActiveHelpRequests(ClassroomStateService.ActiveHelpRequests);
    }

    public async Task ArchiveTeacherQuestion(string questionID)
    {
        ClassroomStateService.ToggleArchivedQuestion(questionID);
        await Clients.All.GetActiveQuestions(ClassroomStateService.ActiveQuestions);
    }

    public async Task SetClassroomState(ClassroomState stateToChangeTo)
    {
        ClassroomStateService.classroomState = stateToChangeTo;
        await Clients.All.GetClassroomState(ClassroomStateService.classroomState);
        await Clients.All.ReceiveMessage("", "Set the classroom state to " + stateToChangeTo, true);
    }
}