using ComputingProject.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor.Services;

Console.WriteLine("Fuck off");

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();

builder.Services.AddSingleton(sp =>
{
    var navigation = sp.GetRequiredService<NavigationManager>();
    return new HubConnectionBuilder()
        .WithUrl(navigation.ToAbsoluteUri("/chatHub"))
        .Build();
});
builder.Services.AddScoped<IChatService, ChatHubClientService>();
builder.Services.AddScoped<IChatHubServer, ChatHubServerProxy>();


await builder.Build().RunAsync();