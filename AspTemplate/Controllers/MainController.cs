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
    }
}