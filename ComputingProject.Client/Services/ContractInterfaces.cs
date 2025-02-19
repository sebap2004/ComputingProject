namespace ComputingProject.Client.Services;

// shared contracts for both client and server
public interface IChatHubClient {
    Task ReceiveMessage(string sender, string content, bool systemMessage);
}

public interface IChatHubServer {
    Task SendMessage(string sender, string content, bool systemMessage);
}