using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        if (model.IsTeacher)
        {
            if (model.Password == "password")
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
            if (model.Password == "password")
            {
                // Sign in
                Console.WriteLine("Successfully logged in as student - Username: " + model.Username + " IsTeacher: " + model.IsTeacher);
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

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("Cookies");
        return Ok();
    }

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
