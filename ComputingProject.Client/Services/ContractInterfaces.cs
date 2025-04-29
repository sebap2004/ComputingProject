using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ComputingProject.Client.Services;

// Responding to Server Messages
public interface IClassroomClient
{
    /// <summary>
    /// Retrieves the current state of a classroom
    /// </summary>
    /// <param name="classroomState">The state of the classroom.</param>
    Task GetClassroomState(ClassroomState classroomState);

    /// <summary>
    /// Retrieves the current list of students from the server.
    /// </summary>
    /// <param name="students">List of students</param>
    Task GetStudents(List<string> students);

    /// <summary>
    /// Retrieves the list of active help requests for a classroom.
    /// </summary>
    /// <param name="students">A list containing help request IDs</param>
    Task GetActiveHelpRequests(List<string> students);

    /// <summary>
    /// Retrieves the list of questions active from the server
    /// </summary>
    /// <param name="questions">List of server questions</param>
    Task GetActiveQuestions(List<TeacherQuestion> questions);

    /// <summary>
    /// Retrieves a list of announcements from the server
    /// </summary>
    /// <param name="announcements">List of announcements</param>
    Task GetAnnouncements(List<TeacherAnnouncement> announcements);

    /// <summary>
    /// Retrieves current task from server
    /// </summary>
    /// <param name="task"></param>
    Task GetCurrentTask(string task);

    /// <summary>
    /// Event function sent when a student joins the classroom
    /// </summary>
    /// <param name="Users">Updated list of users</param>
    Task GetStudentJoinedMessage(List<String> Users);

    /// <summary>
    /// Event function sent when a student leaves the classroom
    /// </summary>
    /// <param name="Users">Updated list of users</param>
    Task GetStudentLeftMessage(List<String> Users);

    /// <summary>
    /// Event function sent when the teacher acknowledges a help request.
    /// </summary>
    Task GetAcknowledgementForHelpRequest();

    /// <summary>
    /// Event function sent when the teacher resolves a help request.
    /// </summary>
    /// <returns></returns>
    Task GetResolutionForHelpRequest();

    /// <summary>
    /// Answers a teacher's question
    /// </summary>
    /// <param name="questionID">ID of the question to answer</param>
    /// <param name="answer">Answer to add to the question</param>
    Task AnswerTeacherQuestion(string questionID, string answer);
}

// Send Message to Server
public interface IClassroomServer
{
    /// <summary>
    /// Gets the current classroom state and sends it to all clients.
    /// </summary>
    Task GetClassroomState();

    /// <summary>
    /// Gets the list of students currently connected to the classroom and sends it to all clients.
    /// </summary>
    Task GetStudents();

    /// <summary>
    /// Gets the list of active help requests and sends it to all clients.
    /// </summary>
    Task GetActiveHelpRequests();

    /// <summary>
    /// Gets the list of active questions and sends it to all clients.   
    /// </summary>
    Task GetActiveQuestions();

    /// <summary>
    /// Gets the current task and sends it to all clients.  
    /// </summary>
    Task GetCurrentTask();

    /// <summary>
    /// Gets the list of announcements and sends it to all clients. 
    /// </summary>
    Task GetAnnouncements();

    /// <summary>
    /// Adds a question to the list of active questions and sends the list to all clients.
    /// </summary>
    /// <param name="question">Question to add into the service</param>
    Task SendTeacherQuestion(TeacherQuestion question);

    /// <summary>
    /// Adds a student's answer to a question and sends the list back to the teacher and the student.
    /// </summary>
    /// <param name="studentID">ID of the student who answered the question</param>
    /// <param name="questionID">ID of the question that was answered</param>
    /// <param name="answer">Answer to the question</param>
    Task AnswerTeacherQuestion(string studentID, string questionID, string answer);

    /// <summary>
    /// Sends a help request to the teacher and adds the student to the list of active help requests.
    /// </summary>
    /// <param name="requestID">ID of the client to store in the help request lists</param>
    Task SendHelpRequest(string requestID);

    /// <summary>
    /// Removes a student from the list of active help requests and sends the list back to the teacher.
    /// </summary>
    /// <param name="requestID">ID of the client to remove from the list</param>
    Task CancelHelpRequest(string requestID);

    /// <summary>
    /// Sends an acknowledgement to the student that a help request has been received and sends the list back to the teacher.
    /// </summary>
    /// <param name="requestID">Client ID to acknowledge</param>
    Task AcknowledgeHelpRequest(string requestID);

    /// <summary>
    /// Sends a resolution to the student that a help request has been resolved and sends the list back to the teacher.
    /// </summary>
    /// <param name="requestID">Client ID to resolve</param>
    Task ResolveHelpRequest(string requestID);

    /// <summary>
    /// Toggles the archived status of a question and sends the list back to the teacher and the student. 
    /// </summary>
    /// <param name="questionID">Question ID to toggle</param>
    Task ArchiveTeacherQuestion(string questionID);

    /// <summary>
    /// Sets the classroom state and sends the new state to all clients.
    /// </summary>
    /// <param name="stateToChangeTo">State to switch to</param>
    Task SetClassroomState(ClassroomState stateToChangeTo);

    /// <summary>
    /// Deletes a question from the list of active questions and sends the list back to all clients.
    /// </summary>
    /// <param name="questionId">Question to delete from the list</param>
    Task DeleteTeacherQuestion(string questionId);

    /// <summary>
    /// Adds an announcement to the list of announcements and sends the list back to all clients.
    /// </summary>
    /// <param name="announcement">Announcement object to add</param>
    Task AddAnnouncement(TeacherAnnouncement announcement);

    /// <summary>
    /// Removes an announcement from the list of announcements and sends the list back to all clients.
    /// </summary>
    /// <param name="announcementId">Announcement object to remove</param>
    Task RemoveAnnouncement(string announcementId);

    /// <summary>
    /// Toggles the hidden status of an announcement and sends the list back to all clients.
    /// </summary>
    /// <param name="announcementId">Announcement object to toggle</param>   
    Task ToggleHideAnnouncement(string announcementId);

    /// <summary>
    /// Sets the current task and sends the new task to all clients.
    /// </summary>
    /// <param name="task">Task string to set to</param>
    Task SetCurrentTask(string task);
}

/// <summary>
/// Classroom service interface providing client side events called by the server
/// </summary>
public interface IClassroomService
{
    /// <summary>
    /// Called when the classroom state changes
    /// </summary>
    event Action<ClassroomState> GetState;
    
    /// <summary>
    /// Called when a student joins
    /// </summary>
    event Action<List<String>> OnStudentJoinedMessage;
    
    /// <summary>
    /// Called when a student leaves
    /// </summary>
    event Action<List<String>> OnStudentLeftMessage;
    
    /// <summary>
    /// Called when the student list is updated
    /// </summary>
    event Action<List<String>> OnReceiveStudentList;
    
    /// <summary>
    /// Called when the active help requests list is updated
    /// </summary>
    event Action<List<String>> OnReceiveActiveHelpRequests;
    
    /// <summary>
    /// Called when the list of active questions is updated
    /// </summary>
    event Action<List<TeacherQuestion>> OnReceiveActiveQuestions;
    
    /// <summary>
    /// Called when the list of announcements is updated.
    /// </summary>
    event Action<List<TeacherAnnouncement>> OnReceiveAnnouncements;
    
    /// <summary>
    /// Called when the current task is updated
    /// </summary>
    event Action<string> OnReceiveCurrentTask;
    
    /// <summary>
    /// Called when a teacher question has been archived
    /// </summary>
    event Action<string> OnArchiveTeacherQuestion;
    
    /// <summary>
    /// Called when a help request has been acknowledged.
    /// </summary>
    event Action OnAcknowledgeHelpRequest;
    
    /// <summary>
    /// Called when a help request has been resolved.
    /// </summary>
    event Action OnResolveHelpRequest;
    
    /// <summary>
    /// Called when a teacher question has been answered
    /// </summary>
    event Action<string> OnAnsweredTeacherQuestion;
    
    /// <summary>
    /// Starts a connection to the classroom
    /// </summary>
    Task StartAsync();
    
    /// <summary>
    /// Stops a connection to the classroom
    /// </summary>
    /// <returns></returns>
    Task StopAsync();
    
    /// <summary>
    /// Method that checks if the user is connected to the server
    /// </summary>
    /// <returns>Boolean representing connection status</returns>
    bool IsConnected();
}

/// <summary>
/// Represents a question posed by a teacher to students.
/// </summary>
public class TeacherQuestion
{
    public TeacherQuestion()
    {
    }

    /// <summary>
    /// Determines whether a student has already answered the question.
    /// </summary>
    /// <param name="studentID">The identifier of the student to check.</param>
    /// <returns>Boolean representing students answer status.</returns>
    public bool StudentAnswered(string studentID)
    {
        return Answers.Any(q => q == studentID);
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="id">ID of question</param>
    /// <param name="questionText">Question being asked.</param>
    public TeacherQuestion(string id, string questionText)
    {
        Id = id;
        QuestionText = questionText;
        Answers = new();
        Archived = false;
    }

    /// <summary>
    /// Constructor. Only used for serialisation.
    /// </summary>
    /// <param name="id">Id of question</param>
    /// <param name="questionText">Question being asked</param>
    /// <param name="answers">Question answers</param>
    /// <param name="archived">if the question is archived</param>
    public TeacherQuestion(string id, string questionText, List<string> answers, bool archived)
    {
        Id = id;
        QuestionText = questionText;
        Answers = answers;
        Archived = archived;
    }

    
    /// <summary>
    /// ID of question
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    ///  Question text that the teacher asked
    /// </summary>
    public string QuestionText { get; set; }

    /// <summary>
    /// Student's answers
    /// </summary>
    public List<string> Answers { get; set; }
    
    /// <summary>
    /// If question is archived
    /// </summary>
    public bool Archived { get; set; }
}


/// <summary>
/// Object representing a teacher's announcement.
/// </summary>
public class TeacherAnnouncement
{
    public TeacherAnnouncement()
    {
    }

    public TeacherAnnouncement(string id, string announcementText)
    {
        Id = id;
        AnnouncementText = announcementText;
        Archived = false;
    }

    /// <summary>
    /// Constructor only used for serialisation
    /// </summary>
    /// <param name="id">ID of announcement</param>
    /// <param name="announcementText">Announcement Text</param>
    /// <param name="archived">Archived status of announcement</param>
    public TeacherAnnouncement(string id, string announcementText, bool archived)
    {
        Id = id;
        AnnouncementText = announcementText;
        Archived = archived;
    }

    /// <summary>
    /// ID of announcement
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Announcement text
    /// </summary>
    public required string AnnouncementText { get; set; }

    /// <summary>
    /// If announcement is hidden or not.
    /// </summary>
    public bool Archived { get; set; }
}