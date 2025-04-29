using MudBlazor.Services;
using ComputingProject.Client.Services;
using ComputingProject.Components;
using ComputingProject.Hubs;
using ComputingProject.Services;
using Microsoft.AspNetCore.Authentication.Cookies;


var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Services.AddSignalR();
builder.Services.AddScoped(sp => new HttpClient 
{ 
});
builder.Services.AddControllers();
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "auth_token";
        options.LoginPath = "/api/auth/login";
        options.Cookie.MaxAge = TimeSpan.FromDays(365);
        options.AccessDeniedPath = "/access-denied";
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IClassroomService, PlaceholderClassroomService>();
builder.Services.AddScoped<IClassroomServer, MockClassroomServer>();
builder.Services.AddSingleton<ClassroomStateService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); 
app.UseRouting();  
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();


app.MapHub<Classroom>("/classroomhub"); 

app.MapControllers();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(ComputingProject.Client._Imports).Assembly);

app.Run();
