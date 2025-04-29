using System.Security.Claims;
using ComputingProject.Client.Services;

namespace ComputingProject.Services;

/// <summary>
/// Classroom state services that manages and updates the state of a virtual classroom, including tracking students, help requests,
/// teacher announcements, and questions.
/// </summary>
public class ClassroomStateService
{
    /// <summary>
    /// Current classroom state enum
    /// </summary>
    public ClassroomState classroomState { get; set; }

    /// <summary>
    /// Current users connected to the hub
    /// </summary>
    public List<string> ConnectedStudents { get; set; } = new();

    /// <summary>
    /// Current active help requests. String is the student's username
    /// </summary>
    public List<string> ActiveHelpRequests { get; set; } = new();

    /// <summary>
    /// Current active questions in the session. Uses teacher question object
    /// </summary>
    public List<TeacherQuestion> ActiveQuestions { get; set; } = new();
    
    /// <summary>
    /// Current active announcements in session.
    /// </summary>
    public List<TeacherAnnouncement> TeacherAnnouncements { get; set; } = new();
    
    /// <summary>
    /// Current task of the classroom
    /// </summary>
    public string CurrentTask { get; set; }

    /// <summary>
    /// Sets the current classroom task
    /// </summary>
    /// <param name="task">Task to set</param>
    public virtual void SetCurrentTask(string task)
    {
        CurrentTask = task;
    }

    /// <summary>
    /// Adds a teacher announcement to the classroom.
    /// </summary>
    /// <param name="announcement">Announcement to add</param>
    public virtual void AddTeacherAnnouncement(TeacherAnnouncement announcement)
    {
        TeacherAnnouncements.Add(announcement);
    }

    /// <summary>
    /// Removes a teacher announcement from the classroom.
    /// </summary>
    /// <param name="announcementID">ID of the announcement to remove</param>
    public virtual void RemoveTeacherAnnouncement(string announcementID)
    {
        TeacherAnnouncement? announcementToRemove = TeacherAnnouncements.FirstOrDefault(a => a.Id == announcementID);
        if (announcementToRemove is not null)
        {
            TeacherAnnouncements.Remove(announcementToRemove);
        }
    }
    
    /// <summary>
    /// Toggles the archived status of a teacher announcement.
    /// </summary>
    /// <param name="announcementID">ID of the announcement to toggle</param>
    public virtual void ToggleHideTeacherAnnouncement(string announcementID)
    {
        TeacherAnnouncement? announcementToRemove = TeacherAnnouncements.FirstOrDefault(a => a.Id == announcementID);
        if (announcementToRemove is not null)
        {
            announcementToRemove.Archived = !announcementToRemove.Archived;
        }
    }
    
    /// <summary>
    /// Adds a student to the student list. 
    /// </summary>
    /// <param name="user">user ID to add to list</param>
    public virtual void AddStudent(string user)
    {
        ConnectedStudents.Add(user);
    }

    /// <summary>
    /// Removes a student to the student list. 
    /// </summary>
    /// <param name="user">user ID to add to list</param>
    public virtual void RemoveStudent(string user)
    {
        ConnectedStudents.Remove(user);
    }

    /// <summary>
    /// Adds a help request to the active help requests list.
    /// </summary>
    /// <param name="user">user that is asking for help</param>
    public virtual void AddHelpRequest(string user)
    {
        ActiveHelpRequests.Add(user);
    }

    /// <summary>
    /// Removes a help request from the active help requests list.
    /// </summary>
    /// <param name="user">student to remove from list</param>
    public virtual void RemoveHelpRequest(string user)
    {
        ActiveHelpRequests.Remove(user);
    }

    /// <summary>
    /// Adds a question to the active questions list.
    /// </summary>
    /// <param name="question">Question to add to list</param>
    public virtual void AddQuestion(TeacherQuestion question)
    {
        ActiveQuestions.Add(question);
        Console.WriteLine("Question added!");
        Console.WriteLine(question.QuestionText);
        Console.WriteLine(question.Id);
        Console.WriteLine(question.Archived);
        Console.WriteLine(question.Answers);

        Console.WriteLine("CURRENT QUESTIONS IN MEMORY:");
        foreach (var q in ActiveQuestions)
        {
            Console.WriteLine("------------------------");
            Console.WriteLine(q.QuestionText);
            Console.WriteLine(q.Id);
            Console.WriteLine(q.Archived);
            Console.WriteLine("     ANSWERS:");
            foreach (var answer in q.Answers)
            {
                Console.WriteLine("     " + answer);
            }
        }
    }

    /// <summary>
    /// Removes a question from the active questions list.
    /// </summary>
    /// <param name="questionId">ID of question to delete</param>
    public virtual void DeleteTeacherQuestion(string questionId)
    {
        TeacherQuestion? questionToRemove = ActiveQuestions.FirstOrDefault(question => question.Id == questionId);
        if (questionToRemove != null)
        {
            ActiveQuestions.Remove(questionToRemove);
        }
        else
        {
            Console.Error.WriteLine("QUESTION NOT FOUND");
        }
    }
    
    
    /// <summary>
    /// Toggles the archived status of a question.
    /// </summary>
    /// <param name="questionID">ID of the question to toggle</param>
    public virtual void ToggleArchivedQuestion(string questionID)
    {
        TeacherQuestion? questionToRemove = ActiveQuestions.FirstOrDefault(question => question.Id == questionID);
        if (questionToRemove != null)
        {
            questionToRemove.Archived = !questionToRemove.Archived;
        }
        else
        {
            Console.Error.WriteLine("QUESTION NOT FOUND");
        }
    }

    /// <summary>
    /// Adds an answer to a question.
    /// </summary>
    /// <param name="questionID">ID of the question to add an answer to</param>
    /// <param name="answer">Answer to add to the question</param>
    public  virtual void AddAnswerToQuestion(string questionID, string answer)
    {
        TeacherQuestion? questionToAnswer = ActiveQuestions.FirstOrDefault(question => question.Id == questionID);
        if (questionToAnswer != null)
        {
            questionToAnswer.Answers.Add(answer);
        }
        else
        {
            Console.Error.WriteLine("QUESTION NOT FOUND");
        }
    }
}