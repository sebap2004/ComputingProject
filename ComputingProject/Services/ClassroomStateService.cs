using System.Security.Claims;
using ComputingProject.Client.Services;

namespace ComputingProject.Services;

public class ClassroomStateService
{
    // Current classroom state enum
    public ClassroomState classroomState { get; set; }
    
    // Current users connected to the hub
    public List<string> ConnectedUsers { get; set; } = new();
    
    // Current active help requests. String is the student's username
    public List<string> ActiveHelpRequests { get; set; } = new();
    
    // Current active questions in the session. Uses teacher object
    public List<TeacherQuestion> ActiveQuestions { get; set; } = new ();

    
    // Adds a student to the student list. String is user ID
    public void AddStudent(string user)
    {
        ConnectedUsers.Add(user);
    }

    // Removes a student to the student list. String is user ID
    public void RemoveStudent(string user)
    {
        ConnectedUsers.Remove(user);
    }
}

// Teacher question object
public class TeacherQuestion
{
    public TeacherQuestion(string question)
    {
        Question = question;
        Answers = new();
    }
    
    // Question text that the teacher asked
    public string Question { get; set; }
    
    // Student's answers
    public List<string> Answers { get; set; } 
}


