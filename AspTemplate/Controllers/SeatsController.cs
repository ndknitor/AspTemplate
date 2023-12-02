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
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] PagingRequest request)
    {
        int total = await context.Seat.Where(s => !s.Deleted).CountAsync();
        Console.WriteLine("Dit me may");
        IEnumerable<RSeat> seats = context.Seat
                                        .Where(s => !s.Deleted)
                                        .OrderBy(s => s.Price)
                                        .Skip((request.Page - 1) * request.Size)
                                        .Take(request.Size)
                                        .Select(s => new RSeat
                                        {
                                            SeatId = s.SeatId,
                                            Name = s.Name,
                                            Price = s.Price,
                                            Bus = new Bus
                                            {
                                                Name = s.Bus.Name,
                                                LicensePlate = s.Bus.LicensePlate,
                                                Seat = null
                                            },
                                        });
        return Ok(new PagingResponse<RSeat>
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
    private static readonly Func<EtdbContext, IEnumerable<int>, IEnumerable<Seat>> GetSeatByIds = EF.CompileQuery
    (
        (EtdbContext context, IEnumerable<int> seatIds) =>
        context.Seat
        .Where(s => seatIds.Contains(s.SeatId))
        .AsSplitQuery()
        .Select(s => new Seat
        {
            SeatId = s.SeatId,
            Name = s.Name,
            Price = s.Price,
            Bus = new Bus
            {
                Name = s.Bus.Name,
                LicensePlate = s.Bus.LicensePlate,
                Seat = null
            },
        })
    );
}
public class RSeat
{
    public int SeatId { get; set; }

    public int Price { get; set; }

    public string Name { get; set; }

    public virtual Bus Bus { get; set; }

}