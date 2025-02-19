using ComputingProject.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor.Services;

Console.WriteLine("Fuck off");

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();

builder.Services.AddScoped(sp =>
{
    var navigation = sp.GetRequiredService<NavigationManager>();
    return new HubConnectionBuilder()
        .WithUrl(navigation.ToAbsoluteUri("/chathub")) // Ensure this URL is correct for your SignalR hub
        .Build();
});



builder.Services.AddScoped<ChatHubClientService>();

builder.Services.AddScoped<ChatHubServerProxy>();

await builder.Build().RunAsync();