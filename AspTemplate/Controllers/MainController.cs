using System.Diagnostics;
using System.Security.Claims;
using AspTemplate.Context;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
namespace NewTemplate.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class MainController : ControllerBase
    {
        private static readonly Func<EtdbContext, IEnumerable<int>, IEnumerable<Seat>> GetSeatByIds = EF.CompileQuery
        (
            (EtdbContext context, IEnumerable<int> seatIds) => context.Seat.Where(s => seatIds.Contains(s.SeatId))
        );
        private static readonly Func<EtdbContext, IEnumerable<int>, IAsyncEnumerable<Seat>> GetSeatByIdsAsync = EF.CompileAsyncQuery
        (
            (EtdbContext context, IEnumerable<int> seatIds) =>
            context.Seat.Where(s => seatIds.Contains(s.SeatId))
        );
        [HttpGet("ids")]
        public IActionResult GetSeatsByIds([FromServices] EtdbContext context, [FromQuery] IEnumerable<int> seatIds)
        {
            return Ok(new RangeResponse<Seat>
            {
                Data = GetSeatByIds(context, seatIds),
                Message = "Get seats successfully"
            });
        }
        [HttpGet("error")]
        public IActionResult GetSeatError([FromServices] EtdbContext context, [FromQuery] IEnumerable<int> seatIds)
        {
            int a = int.Parse("a");
            return Ok(new RangeResponse<Seat>
            {
                Data = GetSeatByIds(context, seatIds),
                Message = "Get seats successfully"
            });
        }
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
        // [HttpGet]
        // //[RemoteCache]
        // public IActionResult Get([FromQuery] GetSeatRequest request, [FromServices] EtdbContext context, [FromServices] IMapper mapper)
        // {
        //     QueryFutureValue<int> total = null;
        //     IEnumerable<RSeat> seats =
        //         mapper.Map<IEnumerable<RSeat>>
        //         (
        //         //context.Seat.Where(s => new int[] { 21, 22, 23 }.Contains(s.SeatId))
        //         //GetSeatByIdsAsync(context, new int[] { 21, 22, 23 })
        //         //GetSeatByIds(context, new int[] { 21, 22, 23 })
        //             context.Seat
        //             .Where(s => s.Deleted == false)
        //             .OrderBy(request.OrderBy, request.Desc)
        //             .DeferredPaginate(request.Page, request.Size, out total)
        //             .SelectExcept(s => new { s.Bus, s.Deleted })
        //         );
        //     return Ok(new PagingResponse<RSeat>
        //     {
        //         Size = request.Size,
        //         Total = total,
        //         Data = seats,
        //         Message = $"There are {total.Value} seats",
        //     });
        // }

    }
}