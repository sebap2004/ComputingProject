﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace ComputingProject.Client.Services;

public class ChatHubClientService : IChatHubClient  
{
    private readonly HubConnection _hubConnection;
    public bool IsConnected => _hubConnection.State == HubConnectionState.Connected;

    public event Action<string, string, bool>? OnMessageReceived;

    public ChatHubClientService(HubConnection hubConnection)
    {
        Console.WriteLine("Added ChatHubClientService");
        _hubConnection = hubConnection;
        _hubConnection.On<string, string, bool>("ReceiveMessage", (user, message, systemMessage) =>
        {
            OnMessageReceived?.Invoke(user, message, systemMessage);
        });
    }

    public async Task StartAsync()
    {
        try
        {
            Console.WriteLine("Starting chat hub connection...");
            await _hubConnection.StartAsync();
            Console.WriteLine("Chat hub connection started successfully. Current state: " + _hubConnection.State);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting chat hub connection: {ex.Message}");
        }
    }

    public async Task StopAsync()
    {
        await _hubConnection.StopAsync();
    }

    public async Task ReceiveMessage(string sender, string content, bool systemMessage)
    {
        OnMessageReceived?.Invoke(sender, content, systemMessage);
    }
}

