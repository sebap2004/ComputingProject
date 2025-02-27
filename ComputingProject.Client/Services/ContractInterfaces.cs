using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ComputingProject.Client.Services;

// Responding to Server Messages
public interface IClassroomClient
{
    Task GetMessage(string sender, string content, bool systemMessage);
    Task GetClassroomState(ClassroomState classroomState);
    Task GetStudents(List<string> students);
    Task GetActiveHelpRequests(List<string> students);
    Task GetActiveQuestions(List<TeacherQuestion> questions);
    Task GetAnnouncements(List<TeacherAnnouncement> announcements);
    Task GetCurrentTask(string task);
    Task GetStudentJoinedMessage(List<String> Users);
    Task GetStudentLeftMessage(List<String> Users);
    Task GetAcknowledgementForHelpRequest();
    Task GetResolutionForHelpRequest();
    Task AnswerTeacherQuestion(string questionID, string answer);
}


// Send Message to Server
public interface IClassroomServer {
    Task SendMessage(string sender, string content, bool systemMessage);
    Task SendMessageToTeacher(string sender, string content, bool systemMessage);
    Task GetClassroomState(string classroomState);
    Task GetStudents();
    Task GetActiveHelpRequests();
    Task GetActiveQuestions();
    Task GetCurrentTask();
    Task GetAnnouncements();
    Task SendTeacherQuestion(TeacherQuestion question);
    Task AnswerTeacherQuestion(string studentID, string questionID, string answer);
    Task SendHelpRequest(string requestID);
    Task CancelHelpRequest(string requestID);
    Task AcknowledgeHelpRequest(string requestID);
    Task ResolveHelpRequest(string requestID);
    Task ArchiveTeacherQuestion(string questionID);
    Task SetClassroomState(ClassroomState stateToChangeTo);
    Task DeleteTeacherQuestion(string questionId);
    Task AddAnnouncement(TeacherAnnouncement announcement);
    Task RemoveAnnouncement(string announcementId);
    Task ToggleHideAnnouncement(string announcementId);
    Task SetCurrentTask(string task);
}

// Client Side Events
public interface IClassroomService
{
    event Action<string, string, bool> OnMessageReceived;
    event Action<ClassroomState> OnJoinGetState;
    event Action<ClassroomState> GetState;
    event Action<List<String>> OnStudentJoinedMessage;
    event Action<List<String>> OnStudentLeftMessage;
    event Action<List<String>> OnReceiveStudentList;
    event Action<List<String>> OnReceiveActiveHelpRequests;
    event Action<List<TeacherQuestion>> OnReceiveActiveQuestions;
    event Action<List<TeacherAnnouncement>> OnReceiveAnnouncements;
    event Action<string> OnReceiveCurrentTask;
    event Action<string> OnArchiveTeacherQuestion;
    event Action OnAcknowledgeHelpRequest;
    event Action OnResolveHelpRequest;
    event Action<string> OnAnsweredTeacherQuestion;
    Task StartAsync();
    Task StopAsync();
    bool IsConnected();
}


public class TeacherQuestion
{
    public TeacherQuestion() {}

    public bool StudentAnswered(string studentID)
    {
        return Answers.Any(q => q == studentID);
    }
    
    public TeacherQuestion(string id, string questionText)
    {
        Id = id;
        QuestionText = questionText;
        Answers = new();
        Archived = false;
    }

    public TeacherQuestion(string id, string questionText, List<string> answers, bool archived)
    {
        Id = id;
        QuestionText = questionText;
        Answers = answers;
        Archived = archived;
    }
    
    public string Id { get; set; }

    // Question text that the teacher asked
    public string QuestionText { get; set; }

    // Student's answers
    public List<string> Answers { get; set; } 
    public bool Archived { get; set; }
}

public class TeacherAnnouncement
{
    public TeacherAnnouncement() {}
    
    public TeacherAnnouncement(string id, string announcementText)
    {
        Id = id;
        AnnouncementText = announcementText;
        Archived = false;
    }

    public TeacherAnnouncement(string id, string announcementText, bool archived)
    {
        Id = id;
        AnnouncementText = announcementText;
        Archived = archived;
    }
    
    public required string Id { get; set; }

    // Announcement
    public required string AnnouncementText { get; set; }
    
    // Hidden or not.
    public bool Archived { get; set; }
}

