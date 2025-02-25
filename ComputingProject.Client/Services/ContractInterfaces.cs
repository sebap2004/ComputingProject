using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ComputingProject.Client.Services;

// Responding to Server Messages
public interface IClassroomClient
{
    Task ReceiveMessage(string sender, string content, bool systemMessage);
    Task GetClassroomState(ClassroomState classroomState);
    Task GetStudents(List<string> students);
    Task GetActiveHelpRequests(List<string> students);
    Task GetActiveQuestions(List<TeacherQuestion> questions);
    Task SendStudentJoinedMessage(List<String> Users);
    Task SendStudentLeftMessage(List<String> Users);
    Task ReceiveAcknowledgementForHelpRequest();
    Task ReceiveResolutionForHelpRequest();
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
    Task SendTeacherQuestion(TeacherQuestion question);
    Task AnswerTeacherQuestion(string studentID, string questionID, string answer);
    Task SendHelpRequest(string requestID);
    Task CancelHelpRequest(string requestID);
    Task AcknowledgeHelpRequest(string requestID);
    Task ResolveHelpRequest(string requestID);
    Task ArchiveTeacherQuestion(string questionID);
    Task SetClassroomState(ClassroomState stateToChangeTo);
    Task DeleteTeacherQuestion(string questionId);
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
    
    public TeacherQuestion(string id, string question)
    {
        Id = id;
        Question = question;
        Answers = new();
        Archived = false;
    }

    public TeacherQuestion(string id, string question, List<string> answers, bool archived)
    {
        Id = id;
        Question = question;
        Answers = answers;
        Archived = archived;
    }
    
    public string Id { get; set; }

    // Question text that the teacher asked
    public string Question { get; set; }

    // Student's answers
    public List<string> Answers { get; set; } 
    public bool Archived { get; set; }
}
