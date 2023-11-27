using AspTemplate.Context;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace NewTemplate.Controllers;
[ApiController]
[Route("/api/[controller]")]
public class SeatsController(EtdbContext context) : ControllerBase
{
    private static readonly Func<EtdbContext, IEnumerable<int>, IEnumerable<Seat>> GetSeatByIds = EF.CompileQuery
    (
        (EtdbContext context, IEnumerable<int> seatIds) =>
        context.Seat.Where(s => seatIds.Contains(s.SeatId))
    );
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] PagingRequest request)
    {
        int total = await context.Seat.Where(s => s.Deleted == false).CountAsync();
        Console.WriteLine("dit me may");
        IEnumerable<Seat> seats = context.Seat.Where(s => s.Deleted == false).OrderBy(s => s.SeatId).Skip((request.Page - 1) * request.Size).Take(request.Size);
        return Ok(new PagingResponse<Seat>
        {
            Size = request.Size,
            Data = seats,
            Message = $"There are {total} seats avalible",
            Total = total
        });
    }
    [HttpGet("range")]
    public IActionResult GetByIds([FromQuery] IEnumerable<int> seatIds)
    {
        return Ok(new RangeResponse<Seat>
        {
            Data = GetSeatByIds(context, seatIds)
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
}