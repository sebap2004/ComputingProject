using System.Security.Claims;
using ComputingProject.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;

namespace ComputingProject.Client.Pages.Views;

public partial class TeacherView : ComponentBase
{
    // Members related to the classroom state
    private List<string> ConnectedStudents { get; set; } = new List<string>();
    public List<string> ActiveHelpRequests { get; set; } = new();
    public List<TeacherQuestion> ActiveQuestions { get; set; } = new ();
    private int StudentCount => ConnectedStudents.Count;
    private ClassroomState CurrentClassroomState;
    private List<Message> messages = new List<Message>();
    private string? SelectQuestionID {get; set;}
    private TeacherQuestion? SelectedQuestionObject { get; set; }
    
    
    private MudTheme Theme = new MudTheme();
    private string UserName { get; set; }
    private string UserRole { get; set; }
    private string QuestionInput { get; set; }
    private Color WorkshopButtonColor => CurrentClassroomState == ClassroomState.Workshop ? Color.Primary : Color.Secondary;
    private Color LectureButtonColor => CurrentClassroomState == ClassroomState.Lecture ? Color.Primary : Color.Secondary;
    private Color ArchiveColor(bool archived) => archived ? Color.Secondary : Color.Warning;
    private Color ViewingColor(bool archived) => archived ? Color.Primary : Color.Info;
    

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
        ClassroomService.OnMessageReceived += OnClassroomServiceOnOnMessageReceived;
        ClassroomService.GetState += OnClassroomServiceOnGetState;
        ClassroomService.OnStudentJoinedMessage += OnClassroomServiceOnOnStudentJoinedMessage;
        ClassroomService.OnStudentLeftMessage += OnClassroomServiceOnOnStudentLeftMessage;
        ClassroomService.OnReceiveStudentList += OnClassroomServiceOnOnReceiveStudentList;
        ClassroomService.OnReceiveActiveQuestions += ReceiveActiveQuestions;
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
            await ClassroomServer.GetActiveQuestions();
        }
        catch (Exception ex)
        {
            // Handle connection errors
            Snackbar.Add("Failed to connect to classroom: " + ex.Message, Severity.Error);
        }
    }

    private void ReceiveActiveQuestions(List<TeacherQuestion> obj)
    {
        ActiveQuestions = obj;
        ShowQuestion(SelectQuestionID);
        Console.WriteLine("Refreshed show question list");
        StateHasChanged();
    }

    private void ShowQuestion(string id)
    {
        SelectQuestionID = id;
        Console.WriteLine("Showing question " + id + "...");
        SelectedQuestionObject = ActiveQuestions.FirstOrDefault(q => q.Id == id);
        StateHasChanged();
    }

    private void OnClassroomServiceOnOnReceiveStudentList(List<string> connectionList)
    {
        ConnectedStudents = connectionList;
        Snackbar.Add("Student List Updated", Severity.Info);
        StateHasChanged();
    }

    private void OnClassroomServiceOnOnMessageReceived(string puser, string pmessage, bool system)
    {
        Message message = new Message() { user = puser, message = pmessage, systemMessage = system };
        messages.Add(message);

        StateHasChanged();
    }

    private void OnClassroomServiceOnOnStudentLeftMessage(List<string> connectionList)
    {
        ConnectedStudents = connectionList;
        Snackbar.Add("Student Left", Severity.Info);
        StateHasChanged();
        Console.WriteLine("Joined Students Count: " + ConnectedStudents.Count);
    }

    private void OnClassroomServiceOnOnStudentJoinedMessage(List<string> connectionList)
    {
        ConnectedStudents = connectionList;
        Snackbar.Add("Student joined", Severity.Success);
        StateHasChanged();
        Console.WriteLine("Joined Students Count: " + ConnectedStudents.Count);
    }

    private void OnClassroomServiceOnGetState(ClassroomState state)
    {
        CurrentClassroomState = state;
        StateHasChanged();
    }

    async Task SendTeacherQuestion()
    {
        Console.WriteLine("Sending Teacher Question, current question text is " + QuestionInput);
        if (string.IsNullOrWhiteSpace(QuestionInput)) return;
        var tempMessage = QuestionInput;
        QuestionInput = string.Empty;
        StateHasChanged();
        TeacherQuestion tempQuestion = new TeacherQuestion(RandomIDGenerator.GenerateRandomID(), tempMessage);
        await ClassroomServer.SendTeacherQuestion(tempQuestion);
    }

    private async Task EnterHandler(KeyboardEventArgs e)
    {
        if (e.Code == "Enter" || e.Code == "NumpadEnter")
        {
            await SendTeacherQuestion();
        }
    }

    private async Task ArchiveQuestion(string questionID)
    {
        await ClassroomServer.ArchiveTeacherQuestion(questionID);
    }
    
    public async Task SetWorkshopState() 
    {
        if (CurrentClassroomState == ClassroomState.Workshop) return;
        await ClassroomServer.SetClassroomState(ClassroomState.Workshop);
    }
    
    public async Task SetLectureState()
    {
        if (CurrentClassroomState == ClassroomState.Lecture) return;
        await ClassroomServer.SetClassroomState(ClassroomState.Lecture);
    }

    public struct Message
    {
        public string user;
        public string message;
        public bool systemMessage;
    }

}