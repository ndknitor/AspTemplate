using System.Diagnostics;
using System.Security.Claims;
using AspTemplate.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace NewTemplate.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class MainController : ControllerBase
    {
        [HttpGet("single")]
        public IActionResult Get()
        {
            float a = 7.11f / 9.32f;
            return Ok(new { date = a.ToString("0.000000000000000000000000000000000") });
        }
        [Authorize(Roles = "User")]
        [HttpGet("authorize")]
        public async Task<IActionResult> GetAuth()
        {
            return Ok();
        }
        [HttpPost("login")]
        public async Task<IActionResult> Post()
        {
            // ClaimsPrincipal principal = new ClaimsPrincipal();
            // ClaimsIdentity identity = new ClaimsIdentity(
            //     new Claim[]
            //     {
            //         new Claim(ClaimTypes.NameIdentifier, userId),
            //         new Claim(ClaimTypes.Role, "User")
            //     },
            //     CookieAuthenticationDefaults.AuthenticationScheme);
            // principal.AddIdentity(identity);
            // await HttpContext.SignInAsync(principal);
            return Ok();
        }
    }
}