using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace ComputingProject.Controllers;

/// <summary>
/// API Controller for authentication.
/// This controller handles the logging in of, retrieval of and logging out of users.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    /// <summary>
    /// Logs a user into the system.
    /// </summary>
    /// <param name="model">Login data transfer object</param>
    /// <returns>Action result indicating the success of the operation.</returns>
    /// <remarks>This is a very simple method of authentication that doesn't check via any database.
    /// All it does is check if the password of the model is equal to a string defined differently for both teacher and student. </remarks>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        if (model.IsTeacher)
        {
            if (model.Password == "iamateacher")
            {
                // Sign in
                Console.WriteLine("Successfully logged in as teacher - Username: " + model.Username + " IsTeacher: " + model.IsTeacher);
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, model.Username),
                    new(ClaimTypes.NameIdentifier, model.Username),
                    new(ClaimTypes.Role, "Teacher")
                };

                var identity = new ClaimsIdentity(claims, "Cookies");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("Cookies", principal);
                return Ok();
            }
            
        }
        else
        {
            if (model.Password == "passwordforstudents")
            {
                // Sign in
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, model.Username),
                    new(ClaimTypes.NameIdentifier, model.Username),
                    new(ClaimTypes.Role, "Student")
                };
                var identity = new ClaimsIdentity(claims, "Cookies");
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync("Cookies", principal);
                return Ok();
            }
        }
        return Unauthorized();
    }

    /// <summary>
    /// Logs a user out of the system.
    /// </summary>
    /// <returns>Action result showing the success of the operation.</returns>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("Cookies");
        return Ok();
    }

    
    /// <summary>
    /// Gets a user's authentication status.
    /// </summary>
    /// <returns>Action result determining whether they are logged in or not.</returns>
    [HttpGet("user")]
    public IActionResult GetUser()
    {
        if (User.Identity.IsAuthenticated)
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            return Ok(claims);
        }
        return Unauthorized();
    }
}