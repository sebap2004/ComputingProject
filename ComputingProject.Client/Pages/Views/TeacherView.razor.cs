using System.Security.Claims;
using ComputingProject.Client.Services;
using ComputingProject.Client.Themes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;

namespace ComputingProject.Client.Pages.Views;

public partial class TeacherView : ComponentBase
{
    // Members related to the classroom state
    private readonly DialogOptions _dialogOptions = new() { FullWidth = true };
    private bool _visible;
    private MudDialog _dialog;
    private MudBreakpointProvider _breakpointProvider;
    private List<string> ConnectedStudents { get; set; } = new List<string>();
    public List<string> ActiveHelpRequests { get; set; } = new();
    public List<TeacherQuestion> ActiveQuestions { get; set; } = new ();
    public List<TeacherAnnouncement> Announcements { get; set; } = new ();
    public string CurrentTask { get; set; }
    private int StudentCount => ConnectedStudents.Count;
    private ClassroomState CurrentClassroomState;
    private List<Message> messages = new List<Message>();
    private string? SelectQuestionID {get; set;}
    private TeacherQuestion? SelectedQuestionObject { get; set; }
    private List<string> AcknowledgedQuestions { get; set; } = new();
    private bool QuestionIsAcknowledged(string questionId) => AcknowledgedQuestions.Contains(questionId);
    private bool IsConnected { get; set; }
    private MudTheme Theme = new DefaultTheme();
    private string UserName { get; set; }
    private string UserRole { get; set; }
    private string QuestionInput { get; set; }
    private string AnnouncementInput { get; set; }
    private string TaskInput { get; set; }
    private Color WorkshopButtonColor => CurrentClassroomState == ClassroomState.Workshop ? Color.Primary : Color.Secondary;
    private Color LectureButtonColor => CurrentClassroomState == ClassroomState.Lecture ? Color.Primary : Color.Secondary;
    private Color ArchiveColor(bool archived) => archived ? Color.Secondary : Color.Warning;
    private Color ViewingColor(bool archived) => archived ? Color.Primary : Color.Info;

    private void SendAcknowledgement(string ID)
    {
        Console.WriteLine("Acknowledging " + ID);
        ClassroomServer.AcknowledgeHelpRequest(ID);
        AcknowledgedQuestions.Add(ID);
        StateHasChanged();
    }
    
    private void SendResolve(string ID)
    {
        Console.WriteLine("Resolving " + ID);
        ClassroomServer.ResolveHelpRequest(ID);
        AcknowledgedQuestions.Remove(ID);
        StateHasChanged();
    }

    async Task SendNewAnnouncement()
    {
        TeacherAnnouncement announcement = new TeacherAnnouncement
        {
            Id =RandomIDGenerator.GenerateRandomID(),
            AnnouncementText = AnnouncementInput
        };
        await ClassroomServer.AddAnnouncement(announcement);
        Console.WriteLine("Sent new announcement " + announcement.AnnouncementText);
        AnnouncementInput = string.Empty;
        StateHasChanged();
    }
    
    private void RemoveAnnouncement(string id)
    {
        ClassroomServer.RemoveAnnouncement(id);
        StateHasChanged();
    }
    
    private void ToggleHideAnnouncement(string id)
    {
        ClassroomServer.ToggleHideAnnouncement(id);
        StateHasChanged();
    }

    async Task SetCurrentTask()
    {
        await ClassroomServer.SetCurrentTask(TaskInput);
        Console.WriteLine("Set current task: " + TaskInput);
        TaskInput = string.Empty;
        StateHasChanged();
    }
    
    async Task ResetTask()
    {
        await ClassroomServer.SetCurrentTask("");
        Console.WriteLine("Reset current task");
        TaskInput = string.Empty;
        StateHasChanged();
    }
    
    protected override async Task OnInitializedAsync()
    {
        IsConnected = false;
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        if (user.Identity!.IsAuthenticated)
        {
            UserName = user.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
            UserRole = user.FindFirst(ClaimTypes.Role)?.Value ?? "No role";
            Console.WriteLine("User role is " + UserRole);
            if (UserRole != "Teacher")
            {
                NavigationManager.NavigateTo($"/access-denied/{Uri.EscapeDataString($"incorrectrole")}");
            }
            await ConnectToHub();
        }
        else
        {
            NavigationManager.NavigateTo($"/access-denied/{Uri.EscapeDataString("notauthenticated")}");
        }
    }

    private async Task ConnectToHub()
    {
        ClassroomService.OnMessageReceived += OnClassroomServiceOnOnMessageReceived;
        ClassroomService.GetState += OnClassroomServiceOnGetState;
        ClassroomService.OnStudentJoinedMessage += OnClassroomServiceOnOnStudentJoinedMessage;
        ClassroomService.OnStudentLeftMessage += OnClassroomServiceOnOnStudentLeftMessage;
        ClassroomService.OnReceiveStudentList += OnClassroomServiceOnOnReceiveStudentList;
        ClassroomService.OnReceiveActiveQuestions += ReceiveActiveQuestions;
        ClassroomService.OnReceiveActiveHelpRequests += ClassroomServiceOnOnReceiveActiveHelpRequests;
        ClassroomService.OnReceiveAnnouncements += ClassroomServiceOnOnReceiveAnnouncements;
        ClassroomService.OnReceiveCurrentTask += ClassroomServiceOnOnReceiveCurrentTask;
        
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
            await ClassroomServer.GetStudents();
            await ClassroomServer.GetActiveQuestions();
            await ClassroomServer.GetActiveHelpRequests();
            await ClassroomServer.GetAnnouncements();
            await ClassroomServer.GetCurrentTask();
            IsConnected = true;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            // Handle connection errors
            Snackbar.Add("Failed to connect to classroom: " + ex.Message, Severity.Error);
        }
    }

    private void ClassroomServiceOnOnReceiveCurrentTask(string obj)
    {
        CurrentTask = obj;
        StateHasChanged();
    }

    private void ClassroomServiceOnOnReceiveAnnouncements(List<TeacherAnnouncement> obj)
    {
        Announcements = obj;
        StateHasChanged();
    }
    
    

    private void ClassroomServiceOnOnReceiveActiveHelpRequests(List<string> obj)
    {
        ActiveHelpRequests = obj;

        foreach (var request in AcknowledgedQuestions.ToList())
        {
            if (!ActiveHelpRequests.Contains(request))
            {
                AcknowledgedQuestions.Remove(request);
            }
        }
        
        StateHasChanged();
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
        if (string.IsNullOrWhiteSpace(id)) return;
        SelectQuestionID = id;
        SelectedQuestionObject = ActiveQuestions.FirstOrDefault(q => q.Id == id);
        StateHasChanged();
        if (_visible)
        {
            Console.WriteLine("Already open");
            return;
        }
        _dialog.ShowAsync();
        _visible = true;
        Console.WriteLine($"Showing question {SelectedQuestionObject.Id}");
    }

    public void DeleteQuestion(string id)
    {
        ClassroomServer.DeleteTeacherQuestion(id);
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

    private async Task QuestionEnterHandler(KeyboardEventArgs e)
    {
        if (e.Code == "Enter" || e.Code == "NumpadEnter")
        {
            await SendTeacherQuestion();
        }
    }
    
    private async Task AnnouncementEnterHandler(KeyboardEventArgs e)
    {
        if (e.Code == "Enter" || e.Code == "NumpadEnter")
        {
            await SendNewAnnouncement();
        }
    }
    
    private async Task TaskEnterHandler(KeyboardEventArgs e)
    {
        if (e.Code == "Enter" || e.Code == "NumpadEnter")
        {
            await SetCurrentTask();
        }
    }

    private void ArchiveQuestion(string questionID)
    {
        ClassroomServer.ArchiveTeacherQuestion(questionID);
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
    
    private void CloseViewAnswers()
    {
        _dialog.CloseAsync();
        _visible = false;
        SelectQuestionID = "";
        SelectedQuestionObject = null;
    }

}