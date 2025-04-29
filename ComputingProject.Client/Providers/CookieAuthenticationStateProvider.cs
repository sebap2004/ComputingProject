using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace ComputingProject.Client.Providers;

/// <summary>
/// Custom authentication state provider that checks cookies to verify authentication status. 
/// </summary>
public class CookieAuthenticationStateProvider : AuthenticationStateProvider
{
    /// <summary>
    /// Http client used to make requests to the API controller
    /// </summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Constructor for the CookieAuthStateProvider class.
    /// Http client included as a parameter for dependency injection.
    /// </summary>
    /// <param name="httpClient">Http client used to make requests about authentication status</param>
    public CookieAuthenticationStateProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    
    /// <summary>
    /// Gets authentication state from the API controller
    /// </summary>
    /// <returns>Authentication state of the user</returns>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/auth/user");
            if (response.IsSuccessStatusCode)
            {
                var claims = await response.Content.ReadFromJsonAsync<List<ClaimDto>>();
                var identity = new ClaimsIdentity(
                    claims?.Select(c => new Claim(c.Type, c.Value)),
                    "cookie"
                );
                var principal = new ClaimsPrincipal(identity);
                return new AuthenticationState(principal);
            }
        }
        catch
        {
            // ignored
        }

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    /// <summary>
    /// Notifies the system that the authentication state has changed.
    /// </summary>
    public void NotifyAuthenticationStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}

/// <summary>
/// Data transfer object representing a claim with a type and a value.
/// </summary>
public class ClaimDto
{
    public string Type { get; set; }
    public string Value { get; set; }
}


/// <summary>
/// Login data transfer object.
/// Used to pass login data to the API controller.
/// </summary>
public class LoginModel
{
    public bool IsTeacher { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}