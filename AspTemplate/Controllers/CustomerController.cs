using System.Diagnostics;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;
namespace NewTemplate.Controllers;
[ApiController]
[Route("/api/[controller]")]
public class CustomerController : ControllerBase
{
    [HttpGet("regular")]
    [Authorize(Policy = nameof(UserPolicy.Regular))]
    public IActionResult GetPage([FromQuery] OffsetPagingRequest request)
    {
        return Ok(Enumerable.Range((request.Page - 1) * request.Size, request.Size));
    }
    [Authorize(Policy = nameof(UserPolicy.VIP))]
    [HttpGet("vip")]
    public IActionResult GetPageVIP([FromQuery] OffsetPagingRequest request)
    {
        return Ok(Enumerable.Range((request.Page - 1) * request.Size, request.Size));
    }
}