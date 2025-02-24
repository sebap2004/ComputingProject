using System.Security.Claims;
using ComputingProject.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;

namespace ComputingProject.Client.Pages.Views;

public partial class TeacherView : ComponentBase
{
    private MudTheme Theme = new MudTheme();
    ClassroomState currentState;
    private List<Message> messages = new List<Message>();
    private string UserName { get; set; }
    private string UserRole { get; set; }
    private string MessageInput;
    
    private Color SeminarButtonColor => currentState == ClassroomState.Seminar ? Color.Primary : Color.Secondary;
    private Color LectureButtonColor => currentState == ClassroomState.Lecture ? Color.Primary : Color.Secondary;
    
    public List<string> JoinedStudents { get; set; } = new List<string>();
    public int StudentCount => JoinedStudents.Count;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        if (user.Identity.IsAuthenticated)
        {
            UserName = user.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
            UserRole = user.FindFirst(ClaimTypes.Role)?.Value ?? "No role";
            Console.WriteLine("User role is " + UserRole);
            if (UserRole != "Teacher")
            {
                NavigationManager.NavigateTo($"/access-denied/{Uri.EscapeDataString($"incorrectrole")}");
            }
            _ = ConnectToHub();
        }
        else
        {
            NavigationManager.NavigateTo($"/access-denied/{Uri.EscapeDataString("notauthenticated")}");
        }
    }

    public async Task ConnectToHub()
    {
        ClassroomService.OnMessageReceived += (puser, pmessage, system) =>
        {
            Message message = new Message()
            {
                user = puser,
                message = pmessage,
                systemMessage = system
            };
            messages.Add(message);
            
            StateHasChanged();
        };

        ClassroomService.GetState += state =>
        {
            currentState = state;
        };

        ClassroomService.OnStudentJoinedMessage += (connectionList) =>
        {
            JoinedStudents = connectionList;
            Snackbar.Add("Student joined", Severity.Success);
            StateHasChanged();
            Console.WriteLine("Joined Students Count: " + JoinedStudents.Count);
        };
        
        ClassroomService.OnStudentLeftMessage += (connectionList) =>
        {
            JoinedStudents = connectionList;
            Snackbar.Add("Student Left", Severity.Info);
            StateHasChanged();
            Console.WriteLine("Joined Students Count: " + JoinedStudents.Count);
        };

        ClassroomService.OnReceiveStudentList += (connectionList) =>
        {
            JoinedStudents = connectionList;
            Snackbar.Add("Student List Updated", Severity.Info);
            StateHasChanged();
        };
        try
        {
            await ClassroomService.StartAsync();
            while (!ClassroomService.IsConnected())
            {
                await Task.Delay(100);
            }
            Snackbar.Clear();
            Snackbar.Add("Joined Classroom Successfully", Severity.Success);
            await ClassroomServer.GetClassroomState(UserName);
            StateHasChanged();
            await ClassroomServer.GetStudents();
            
        }
        catch (Exception ex)
        {
            // Handle connection errors
            Snackbar.Add("Failed to connect to classroom: " + ex.Message, Severity.Error);
        }
    }

    async Task Send()
    {
        if (string.IsNullOrWhiteSpace(MessageInput)) return;
    
        var tempMessage = MessageInput;
        MessageInput = string.Empty;
        StateHasChanged();

        await ClassroomServer.SendMessage(UserName, tempMessage, false);
    }

    private async Task Enter(KeyboardEventArgs e)
    {
        if (e.Code == "Enter" || e.Code == "NumpadEnter")
        {
            await Send();
        }
    }
    
    public async Task SetSeminarState() 
    {
        if (currentState == ClassroomState.Seminar) return;
        await ClassroomServer.SetClassroomState(ClassroomState.Seminar);
    }
    
    public async Task SetLectureState()
    {
        if (currentState == ClassroomState.Lecture) return;
        await ClassroomServer.SetClassroomState(ClassroomState.Lecture);
    }

    public struct Message
    {
        public string user;
        public string message;
        public bool systemMessage;
    }

}