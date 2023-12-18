using AspTemplate.Context;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace NewTemplate.Controllers;
[ApiController]
[Route("/api/[controller]")]
public class SeatsController(EtdbContext context, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] PagingRequest request)
    {
        var total = await context.Seat.Where(s => !s.Deleted).CountAsync();
        var seats = context.Seat
                            .Where(s => !s.Deleted)
                            .OrderBy(s => s.Price)
                            .Skip(request.Offset)
                            .Take(request.Size)
                            .Select(mapper.Map<RSeat>);
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
            Data = GetSeatByIds(context, seatIds),
            Message = "Get seat range"
        });
    }
    [HttpGet("error")]
    public IActionResult GetError([FromQuery] IEnumerable<int> seatIds)
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
        .Select(s => new Seat
        {
            SeatId = s.SeatId,
            Name = s.Name,
            Price = s.Price
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