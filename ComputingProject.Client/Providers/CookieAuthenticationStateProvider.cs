using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

public class CookieAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;

    public CookieAuthenticationStateProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            // Call the API to check if the user is authenticated
            var response = await _httpClient.GetAsync("api/auth/user");
            if (response.IsSuccessStatusCode)
            {
                // Deserialize the claims (adjust based on your API response)
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
            // Handle network errors gracefully
        }
        // Return unauthenticated state if the API call fails or returns 401
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public void NotifyAuthenticationStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}

public class ClaimDto
{
    public string Type { get; set; }
    public string Value { get; set; }
}

public class LoginModel
{
    public bool IsTeacher { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}