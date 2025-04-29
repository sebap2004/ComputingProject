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
    /// <summary>
    /// Dialog options for the dialog that opens when viewing answers to a question
    /// </summary>
    private readonly DialogOptions _dialogOptions = new() { FullWidth = true };
    /// <summary>
    /// Visibility of the question dialog
    /// </summary>
    private bool _visible;
    /// <summary>
    /// Property binding to the question dialog
    /// </summary>
    private MudDialog _dialog;
    /// <summary>
    /// Breakpoint provider to react to browser size, triggering render changes.
    /// </summary>
    private MudBreakpointProvider _breakpointProvider;
    /// <summary>
    /// Local cached list of students. Updated by the server
    /// </summary>
    private List<string> ConnectedStudents { get; set; } = new List<string>();
    
    /// <summary>
    /// Local cached list of active help requests. Updated by the server
    /// </summary>
    private List<string> ActiveHelpRequests { get; set; } = new();
    
    /// <summary>
    /// Local cached list of active questions. Updated by the server
    /// </summary>
    private List<TeacherQuestion> ActiveQuestions { get; set; } = new ();
    
    /// <summary>
    /// Local cached list of announcements. Updated by the server
    /// </summary>
    private List<TeacherAnnouncement> Announcements { get; set; } = new ();
    
    /// <summary>
    /// Local cached current task. Updated by the server
    /// </summary>
    public string CurrentTask { get; set; }
    
    /// <summary>
    /// Expression bound property that represents connected students.
    /// </summary>
    private int StudentCount => ConnectedStudents.Count;
    
    /// <summary>
    /// Local cached state of the classroom. Updated by the server.
    /// </summary>
    private ClassroomState CurrentClassroomState;
    
    /// <summary>
    /// The current question ID selected and displayed in the dialog.
    /// </summary>
    private string? SelectQuestionID {get; set;}
    
    /// <summary>
    /// The current question object selected and displayed in the dialog.
    /// </summary>
    private TeacherQuestion? SelectedQuestionObject { get; set; }
    
    /// <summary>
    /// List of question IDs that have been acknowledged by the teacher.
    /// </summary>
    private List<string> AcknowledgedQuestions { get; set; } = new();
    /// <summary>
    /// Expression bound property that checks the acknowledgement status of a question.
    /// </summary>
    /// <param name="questionId">ID of question to check</param>
    /// <returns>If the question is acknowledged or not</returns>
    private bool QuestionIsAcknowledged(string questionId) => AcknowledgedQuestions.Contains(questionId);
    /// <summary>
    /// If the teacher is connected to the classroom.
    /// </summary>
    private bool IsConnected { get; set; }
    
    /// <summary>
    /// Theme of the page.
    /// </summary>
    private MudTheme Theme = new DefaultTheme();
    
    /// <summary>
    /// Cached username retrieved from authentication state provider.
    /// </summary>
    private string UserName { get; set; }
    /// <summary>
    /// Cached user role retrieved from authentication state provider.
    /// </summary>
    private string UserRole { get; set; }
    
    /// <summary>
    /// Question input property bound to the Question text input box
    /// </summary>
    private string QuestionInput { get; set; }
    
    /// <summary>
    /// Announcement input property bound to the Announcement text input box
    /// </summary>
    private string AnnouncementInput { get; set; }
    
    /// <summary>
    /// Task input property bound to the Task text input box
    /// </summary>
    private string TaskInput { get; set; }
    
    /// <summary>
    /// Color of the workshop button, dependent on the current classroom state.
    /// </summary>
    private Color WorkshopButtonColor => CurrentClassroomState == ClassroomState.Workshop ? Color.Primary : Color.Secondary;
    
    /// <summary>
    /// Color of the lecture button, dependent on the current classroom state.
    /// </summary>
    private Color LectureButtonColor => CurrentClassroomState == ClassroomState.Lecture ? Color.Primary : Color.Secondary;
    
    /// <summary>
    /// Color of the archive button of a question or announcement, dependent on the archived status of the question.
    /// </summary>
    /// <param name="archived">Whether the question is archived</param>
    /// <returns>Color to use</returns>
    private Color ArchiveColor(bool archived) => archived ? Color.Secondary : Color.Warning;
    
    /// <summary>
    /// Color of the viewing button of a question, dependent on the viewing status of the question.
    /// </summary>
    /// <param name="viewing">If the question is being viewed or not</param>
    /// <returns>Color to use</returns>
    private Color ViewingColor(bool viewing) => viewing ? Color.Primary : Color.Info;

    /// <summary>
    /// Sends acknowledgement of a help request to the server.
    /// </summary>
    /// <param name="ID">ID of request to acknowledge</param>
    private void SendAcknowledgement(string ID)
    {
        Console.WriteLine("Acknowledging " + ID);
        ClassroomServer.AcknowledgeHelpRequest(ID);
        AcknowledgedQuestions.Add(ID);
        StateHasChanged();
    }
    
    /// <summary>
    /// Sends resolution of a help request to the server.
    /// </summary>
    /// <param name="ID">ID of request to resolve</param>
    private void SendResolve(string ID)
    {
        Console.WriteLine("Resolving " + ID);
        ClassroomServer.ResolveHelpRequest(ID);
        AcknowledgedQuestions.Remove(ID);
        StateHasChanged();
    }

    /// <summary>
    /// Sends a new announcement to the server.
    /// </summary>
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
    
    /// <summary>
    /// Removes an announcement from the server.
    /// </summary>
    /// <param name="id">ID of announcement to remove</param>
    private void RemoveAnnouncement(string id)
    {
        ClassroomServer.RemoveAnnouncement(id);
        StateHasChanged();
    }
    
    /// <summary>
    /// Toggles the archived status of an announcement.
    /// </summary>
    /// <param name="id">ID of announcement to toggle</param>
    private void ToggleHideAnnouncement(string id)
    {
        ClassroomServer.ToggleHideAnnouncement(id);
        StateHasChanged();
    }

    /// <summary>
    /// Sets the current task of the classroom.
    /// </summary>
    async Task SetCurrentTask()
    {
        await ClassroomServer.SetCurrentTask(TaskInput);
        Console.WriteLine("Set current task: " + TaskInput);
        TaskInput = string.Empty;
        StateHasChanged();
    }
    
    /// <summary>
    /// Resets the current task of the classroom.
    /// </summary>
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

    /// <summary>
    /// Connects to the virtual classroom. called when the page is initialised.
    /// </summary>
    private async Task ConnectToHub()
    {
        ClassroomService.GetState += OnClassroomStateChanged;
        ClassroomService.OnStudentJoinedMessage += OnStudentJoinedMessage;
        ClassroomService.OnStudentLeftMessage += OnStudentLeftMessage;
        ClassroomService.OnReceiveStudentList += ReceiveStudentListFromClassroomService;
        ClassroomService.OnReceiveActiveQuestions += ReceiveActiveQuestions;
        ClassroomService.OnReceiveActiveHelpRequests += ReceiveActiveHelpRequestsFromServer;
        ClassroomService.OnReceiveAnnouncements += ReceiveAnnouncementsFromClassroomService;
        ClassroomService.OnReceiveCurrentTask += ReceiveTaskFromClassroomService;
        
        try
        {
            await ClassroomService.StartAsync();
            while (!ClassroomService.IsConnected())
            {
                await Task.Delay(100);
            }
            Snackbar.Clear();
            Snackbar.Add("Joined Classroom Successfully", Severity.Success);
            await ClassroomServer.GetClassroomState();
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

    /// <summary>
    /// Event function called when the server's current task is updated.
    /// </summary>
    /// <param name="obj">Task from server</param>
    private void ReceiveTaskFromClassroomService(string obj)
    {
        CurrentTask = obj;
        StateHasChanged();
    }

    /// <summary>
    /// Event function called when the server's announcement list is updated.
    /// </summary>
    /// <param name="obj">List of announcement from server</param>
    private void ReceiveAnnouncementsFromClassroomService(List<TeacherAnnouncement> obj)
    {
        Announcements = obj;
        StateHasChanged();
    }
    
    
    /// <summary>
    /// Event function called when the server's help request list is updated.
    /// </summary>
    /// <param name="obj"></param>
    private void ReceiveActiveHelpRequestsFromServer(List<string> obj)
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

    /// <summary>
    /// Event function called when the server's active questions list is updated.
    /// </summary>
    /// <param name="obj"></param>
    private void ReceiveActiveQuestions(List<TeacherQuestion> obj)
    {
        ActiveQuestions = obj;
        StateHasChanged();
    }

    /// <summary>
    /// Displays a question in a dialog box.
    /// Shows the question text and a list of student answers.
    /// </summary>
    /// <param name="id">ID of the question to show.</param>
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

    
    /// <summary>
    /// Deletes a question from the server.
    /// </summary>
    /// <param name="id">ID of the question to delete</param>
    public void DeleteQuestion(string id)
    {
        ClassroomServer.DeleteTeacherQuestion(id);
    }
    
    /// <summary>
    /// Called when the list of connected students on the server is updated.
    /// </summary>
    /// <param name="connectionList">List of connected students</param>
    private void ReceiveStudentListFromClassroomService(List<string> connectionList)
    {
        ConnectedStudents = connectionList;
        Snackbar.Add("Student List Updated", Severity.Info);
        StateHasChanged();
    }
    
    /// <summary>
    /// Called when a student leaves the classroom.
    /// </summary>
    /// <param name="connectionList">New list of students</param>
    private void OnStudentLeftMessage(List<string> connectionList)
    {
        ConnectedStudents = connectionList;
        Snackbar.Add("Student Left", Severity.Info);
        StateHasChanged();
        Console.WriteLine("Joined Students Count: " + ConnectedStudents.Count);
    }

    /// <summary>
    /// Called when a student joins the classroom
    /// </summary>
    /// <param name="connectionList">New list of students</param>
    private void OnStudentJoinedMessage(List<string> connectionList)
    {
        ConnectedStudents = connectionList;
        Snackbar.Add("Student joined", Severity.Success);
        StateHasChanged();
        Console.WriteLine("Joined Students Count: " + ConnectedStudents.Count);
    }

    /// <summary>
    /// Called when the classroom state has changed.
    /// </summary>
    /// <param name="state">State to change to</param>
    private void OnClassroomStateChanged(ClassroomState state)
    {
        CurrentClassroomState = state;
        StateHasChanged();
    }

    /// <summary>
    /// Sends a question to the server for student to answer
    /// </summary>
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

    /// <summary>
    /// Keyboard event handler for a question object. Sends a teacher question if the key pressed was enter.
    /// </summary>
    /// <param name="e">Keyboard Event Object</param>
    private async Task QuestionEnterHandler(KeyboardEventArgs e)
    {
        if (e.Code == "Enter" || e.Code == "NumpadEnter")
        {
            await SendTeacherQuestion();
        }
    }
    
    /// <summary>
    /// Keyboard event handler for an announcement object. Sends an announcement if the key pressed was enter.
    /// </summary>
    /// <param name="e">Keyboard Event Object</param>
    private async Task AnnouncementEnterHandler(KeyboardEventArgs e)
    {
        if (e.Code == "Enter" || e.Code == "NumpadEnter")
        {
            await SendNewAnnouncement();
        }
    }
    
    /// <summary>
    /// Keyboard event handler for a task object. Sends a new task if the key pressed was enter.
    /// </summary>
    /// <param name="e">Keyboard Event Object</param>
    private async Task TaskEnterHandler(KeyboardEventArgs e)
    {
        if (e.Code == "Enter" || e.Code == "NumpadEnter")
        {
            await SetCurrentTask();
        }
    }

    /// <summary>
    /// Toggles archived status of a question.
    /// </summary>
    /// <param name="questionID">ID of the question to toggle</param>
    private void ArchiveQuestion(string questionID)
    {
        ClassroomServer.ArchiveTeacherQuestion(questionID);
    }
    
    /// <summary>
    /// Sets the state of the classroom to the workshop state.
    /// </summary>
    public async Task SetWorkshopState() 
    {
        if (CurrentClassroomState == ClassroomState.Workshop) return;
        await ClassroomServer.SetClassroomState(ClassroomState.Workshop);
    }
    
    /// <summary>
    /// Sets the state of the classroom to the lecture state
    /// </summary>
    public async Task SetLectureState()
    {
        if (CurrentClassroomState == ClassroomState.Lecture) return;
        await ClassroomServer.SetClassroomState(ClassroomState.Lecture);
    }
    
    
    /// <summary>
    /// Closes the question dialog.
    /// </summary>
    private void CloseViewAnswers()
    {
        _dialog.CloseAsync();
        _visible = false;
        SelectQuestionID = "";
        SelectedQuestionObject = null;
    }

}