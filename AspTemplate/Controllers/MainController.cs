using System.Diagnostics;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;
namespace AspTemplate.Controllers;
[ApiController]
[Route("/api/[controller]")]
public class MainController : ControllerBase
{
    [HttpGet]
    [SwaggerOperation($"Roles: {nameof(Role.Public)}")]
    public IActionResult GetPage([FromQuery] OffsetPagingRequest request)
    {
        return Ok(Enumerable.Range((request.Page - 1) * request.Size, request.Size));
    }
    [HttpGet("single")]
    [SwaggerOperation($"Roles: {nameof(Role.Public)}")]
    public IActionResult Get()
    {
        float a = 7.11f / 9.32f;
        return Ok(new { date = a.ToString("0.000000000000000000000000000000000") });
    }
    [HttpGet("option")]
    [SwaggerOperation($"Roles: {nameof(Role.Public)}")]
    public IActionResult Get([FromServices] IOptions<ExampleOption> option)
    {
        return Ok(new { value = option.Value.ExampleValue });
    }
    [HttpGet("date")]
    [SwaggerOperation($"Roles: {nameof(Role.Public)}")]
    public IActionResult GetDate()
    {
        return Ok(new { date = DateTime.Now });
    }
    [HttpGet("hostname")]
    [SwaggerOperation($"Roles: {nameof(Role.Public)}")]
    public IActionResult GetHostname()
    {
        return Ok(Dns.GetHostName());
    }
    [HttpGet("environment")]
    public IActionResult GetEnvironment([FromServices] IWebHostEnvironment environment, [FromServices] IConfiguration configuration)
    {
        return Ok(new { environment, connectionString = configuration.GetConnectionString("Default") });
    }
    [HttpGet("error")]
    public IActionResult GetError()
    {
        int a = int.Parse("a");
        return Ok();
    }
}