using ComputingProject.Client.Providers;
using ComputingProject.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using MudBlazor.Services;

Console.WriteLine("Fuck off");

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) 
});

builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
});

builder.Services.AddSingleton(sp =>
{
    var navigation = sp.GetRequiredService<NavigationManager>();
    return new HubConnectionBuilder()
        .WithUrl(navigation.ToAbsoluteUri("/classroomhub"))
        .Build();
});



builder.Services.AddScoped<IClassroomService, ClassroomClientService>();
builder.Services.AddScoped<IClassroomServer, ClassroomServerProxy>();





await builder.Build().RunAsync();