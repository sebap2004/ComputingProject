using System.Security.Claims;
using ComputingProject.Client.Services;

namespace ComputingProject.Services;

public class ClassroomStateService
{
    public ClassroomState classroomState { get; set; }
    public List<String> Users { get; set; } = new List<string>();

    public void AddStudent(String user)
    {
        Users.Add(user);
        Console.WriteLine($"Added student {user}, new count {Users.Count}");
    }

    public void RemoveStudent(String user)
    {
        Users.Remove(user);
    }
}

