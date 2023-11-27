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
public class AuthController(IHttpContextAccessor accessor, IConfiguration configuration) : ControllerBase
{
    [HttpPost("login/cookie")]
    public async Task<IActionResult> LoginCookie()
    {
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
    private string GetJwt(IEnumerable<Claim> claims)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtProvider:SecretKey"]));
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