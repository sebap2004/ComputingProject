using System.Security.Claims;
using ComputingProject.Client.Services;

namespace ComputingProject.Services;

public class ClassroomStateService
{
    // Current classroom state enum
    public ClassroomState classroomState { get; set; }

    // Current users connected to the hub
    public List<string> ConnectedStudents { get; set; } = new();

    // Current active help requests. String is the student's username
    public List<string> ActiveHelpRequests { get; set; } = new();

    // Current active questions in the session. Uses teacher object
    public List<TeacherQuestion> ActiveQuestions { get; set; } = new();
    
    // Current active announcements in session.
    public List<TeacherAnnouncement> TeacherAnnouncements { get; set; } = new();
    
    public string CurrentTask { get; set; }

    public void SetCurrentTask(string task)
    {
        CurrentTask = task;
    }

    public void AddTeacherAnnouncement(TeacherAnnouncement announcement)
    {
        TeacherAnnouncements.Add(announcement);
    }

    public void RemoveTeacherAnnouncement(string announcementID)
    {
        TeacherAnnouncement? announcementToRemove = TeacherAnnouncements.FirstOrDefault(a => a.Id == announcementID);
        if (announcementToRemove is not null)
        {
            TeacherAnnouncements.Remove(announcementToRemove);
        }
    }
    
    public void ToggleHideTeacherAnnouncement(string announcementID)
    {
        TeacherAnnouncement? announcementToRemove = TeacherAnnouncements.FirstOrDefault(a => a.Id == announcementID);
        if (announcementToRemove is not null)
        {
            announcementToRemove.Archived = !announcementToRemove.Archived;
        }
    }
    
    // Adds a student to the student list. String is user ID
    public void AddStudent(string user)
    {
        ConnectedStudents.Add(user);
    }

    // Removes a student to the student list. String is user ID
    public void RemoveStudent(string user)
    {
        ConnectedStudents.Remove(user);
    }

    public void AddHelpRequest(string user)
    {
        ActiveHelpRequests.Add(user);
    }

    public void RemoveHelpRequest(string user)
    {
        ActiveHelpRequests.Remove(user);
    }

    public void AddQuestion(TeacherQuestion question)
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

    public void DeleteTeacherQuestion(string questionId)
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
    
    public void ToggleArchivedQuestion(string questionID)
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

    public void AddAnswerToQuestion(string questionID, string answer)
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