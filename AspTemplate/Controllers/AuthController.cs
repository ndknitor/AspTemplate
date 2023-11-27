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
    public async Task<IActionResult> Post()
    {
        ClaimsPrincipal principal = new ClaimsPrincipal();
        ClaimsIdentity identity = new ClaimsIdentity(
            new Claim[]
            {
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    new Claim(ClaimTypes.Role, "User")
            },
            CookieAuthenticationDefaults.AuthenticationScheme);
        principal.AddIdentity(identity);
        await accessor.HttpContext.SignInAsync(principal);
        return Ok(new StandardResponse
        {
            Message = "Authenticate successfully"
        });
    }
    [HttpPost("login/jwt")]
    public IActionResult LoginJWT()
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtProvider:SecretKey"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Role, "User")
        };
        var token = new JwtSecurityToken(
        configuration["JwtProvider:Issuer"],
        configuration["JwtProvider:Audience"],
        claims,
        expires: DateTime.Now.AddHours(int.Parse(configuration["AuthenticationExpireHours"])),
        signingCredentials: credentials);
        return Ok(new SingleResponse<string>
        {
            Message = "Authenticate successfully",
            Data = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }
    [Authorize]
    [HttpGet("authorize")]
    public IActionResult GetAuth()
    {
        return Ok(accessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}