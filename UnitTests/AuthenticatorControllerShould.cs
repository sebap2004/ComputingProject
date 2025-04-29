using System.Security.Claims;
using ComputingProject.Client.Providers;
using ComputingProject.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
namespace UnitTests;

public class AuthenticationControllerShould
{
    private Mock<IAuthenticationService> authServiceMock;
    private Mock<IServiceProvider> serviceProviderMock;
    
    public AuthenticationControllerShould()
    {
        authServiceMock = new Mock<IAuthenticationService>();
        authServiceMock
            .Setup(a => a.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);
        serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(s => s.GetService(typeof(IAuthenticationService)))
            .Returns(authServiceMock.Object);       
    }
    
    
    [Fact]
    public async Task Teacher_OnLoginSubmit_ShouldReturnOk()
    {
        var controller = new AuthController()
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext {
                    RequestServices = serviceProviderMock.Object
                }
            }
        };

        var result = controller.Login(new LoginModel()
        {
            Username = "test",
            Password = "iamateacher",
            IsTeacher = true
        });
        
        // assert
        var okResult = await result as OkResult;
        Assert.NotNull(okResult);
        Assert.Equal(200, okResult.StatusCode);
        Assert.NotNull(okResult);
        authServiceMock.Verify(a => a.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()));
    }
    
    
    [Fact]
    public async Task Teacher_OnLoginSubmitWithBadData_ShouldReturnUnauthorized()
    {
        var controller = new AuthController()
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext {
                    RequestServices = serviceProviderMock.Object
                }
            }
        };

        var result = controller.Login(new LoginModel()
        {
            Username = "test",
            Password = "badlogin",
            IsTeacher = true
        });
        
        // assert
        var unauthorizedResult = await result as UnauthorizedResult;
        Assert.NotNull(unauthorizedResult);
        Assert.Equal(401, unauthorizedResult.StatusCode);
        Assert.NotNull(unauthorizedResult);
    }
    
    
    [Fact]
    public async Task Student_OnLoginSubmit_ShouldReturnOk()
    {
        var controller = new AuthController()
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext {
                    RequestServices = serviceProviderMock.Object
                }
            }
        };

        var result = controller.Login(new LoginModel()
        {
            Username = "test",
            Password = "passwordforstudents",
            IsTeacher = false
        });
        
        // assert
        var okResult = await result as OkResult;
        Assert.NotNull(okResult);
        Assert.Equal(200, okResult.StatusCode);
        Assert.NotNull(okResult);
        authServiceMock.Verify(a => a.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()));
    }
    
    [Fact]
    public async Task Student_OnLoginSubmitWithBadData_ShouldReturnUnauthorized()
    {
        var controller = new AuthController()
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext {
                    RequestServices = serviceProviderMock.Object
                }
            }
        };

        var result = controller.Login(new LoginModel()
        {
            Username = "test",
            Password = "incorrectpassword",
            IsTeacher = false
        });
        
        // assert
        var okResult = await result as UnauthorizedResult;
        Assert.NotNull(okResult);
        Assert.Equal(401, okResult.StatusCode);
        Assert.NotNull(okResult);
    }
    
    
    [Fact]
    public async Task GetUser_OnSuccess_ShouldReturnClaims()
    {
        //arrange
        var claims = new List<Claim>() 
        { 
            new(ClaimTypes.Name, "username"),
            new(ClaimTypes.NameIdentifier, "userId"),
            new("name", "Daniel"),
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        
        var controller = new AuthController()
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext {
                    RequestServices = serviceProviderMock.Object,
                    User = new ClaimsPrincipal(identity)
                },
            }
        }; 
        
        //act
        var result = controller.GetUser();
      
        //assert
        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.Equal(200, okResult.StatusCode);
        Assert.NotNull(okResult.Value);
    }
    
    /// <summary>
    /// Initialisation of controller adapted from (HeroWong, 2021).
    /// When logout out method is called
    /// Verifies that the correct methods — SignOutAsync (mocked services) — are called
    /// Check that status is ok (200) 
    /// </summary>
    [Fact]
    public async Task Logout_OnSuccess_ShouldReturnOk()
    {
        //arrange
        authServiceMock
            .Setup(_ => _.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);
        var controller = new AuthController()
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext {
                    RequestServices = serviceProviderMock.Object
                }
            }
        };
        
        //act
        var result = await controller.Logout();
      
        //assert
        var okResult = result as OkResult;
        Assert.NotNull(okResult);
        Assert.Equal(200, okResult.StatusCode);
        authServiceMock.Verify(a => a.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()));
    }
}