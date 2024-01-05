using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace NewTemplate.Controllers;
[ApiController]
[Route("/api/[controller]")]
public class AuthController(IHttpContextAccessor accessor, IConfiguration configuration, IWebHostEnvironment environment) : ControllerBase
{
    [HttpGet("debug/cookie/{id:long}")]
    public async Task<IActionResult> DebugCookie([FromRoute] long id)
    {
        if (environment.IsProduction())
        {
            return NotFound(null);
        }
        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, id.ToString()),
            new Claim(ClaimTypes.Role, "User")
        };
        await GetCookie(claims);
        return Ok(new StandardResponse
        {
            Message = "Authenticate successfully"
        });
    }
    [HttpGet("debug/jwt/{id:long}")]
    public IActionResult DebugJwt([FromRoute] long id)
    {
        if (environment.IsProduction())
        {
            return NotFound(null);
        }
        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, id.ToString()),
            new Claim(ClaimTypes.Role, "User")
        };
        return Ok(new SingleResponse<string>
        {
            Data = GetJwt(claims),
            Message = "Authenticate successfully"
        });
    }
    [HttpGet("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        HttpContext context = accessor.HttpContext;
        if (context.User.Identity.AuthenticationType == CookieAuthenticationDefaults.AuthenticationScheme)
        {
            await context.SignOutAsync();
            return Ok(new StandardResponse
            {
                Message = "Logout successfully"
            });
        }
        else
        {
            return Forbid();
        }
    }
    [HttpPost("login/cookie")]
    public async Task<IActionResult> LoginCookie()
    {
        if (HaveAuthorizationHeader())
        {
            return BadRequest(new StandardResponse
            {
                Message = "Authorization headers are not allowed"
            });
        }
        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Role, "User")
        };
        await GetCookie(claims);
        return Ok(new StandardResponse
        {
            Message = "Authenticate successfully"
        });
    }
    [HttpPost("login/jwt")]
    public IActionResult LoginJwt()
    {
        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Role, "User")
        };
        return Ok(new SingleResponse<string>
        {
            Message = "Authenticate successfully",
            Data = GetJwt(claims)
        });
    }
    [HttpGet("refresh")]
    [Authorize]
    public async Task<IActionResult> Refresh()
    {
        SingleResponse<string> response = new SingleResponse<string>
        {
            Message = "Refresh successfully"
        };
        HttpContext context = accessor.HttpContext;
        var claims = context.User.Claims;
        if (context.User.Identity.AuthenticationType == CookieAuthenticationDefaults.AuthenticationScheme)
        {
            await GetCookie(claims);
            return Ok(response);
        }
        else
        {
            response.Data = GetJwt(claims);
            return Ok(response);
        }
    }
    [HttpGet("authorize")]
    [Authorize]
    public IActionResult GetAuth()
    {
        return Ok(accessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
    }
    [HttpPost("test")]
    public IActionResult Test([FromBody] SignInRequest request)
    {
        return Ok();
    }
    private bool HaveAuthorizationHeader()
    {
        string authorization = accessor.HttpContext.Request.Headers["Authorization"];
        return !string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer ");
    }
    private string GetJwt(IEnumerable<Claim> claims)
    {
        var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(configuration["JwtProvider:SecretKey"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
        configuration["JwtProvider:Issuer"],
        configuration["JwtProvider:Audience"],
        claims,
        expires: DateTime.Now.AddHours(int.Parse(configuration["AuthenticationExpireHours"])),
        signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    private async Task GetCookie(IEnumerable<Claim> claims)
    {
        ClaimsPrincipal principal = new ClaimsPrincipal();
        ClaimsIdentity identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);
        principal.AddIdentity(identity);
        await accessor.HttpContext.SignInAsync(principal);
    }
}

public class SignInRequest
{
    [Required]
    [EmailAddress]
    [MinLength(6)]
    [MaxLength(128)]
    public string Email { get; set; }
    [Required]
    [MinLength(8)]
    [MaxLength(2048)]
    public string Password { get; set; }
}