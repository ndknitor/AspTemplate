using System.ComponentModel.DataAnnotations;
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
                            .OrderBy(s => s.SeatId)
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

    [HttpPost]
    public async Task<IActionResult> Insert([FromBody][Required][MaxLength(128)] IEnumerable<CSeat> request)
    {
        IEnumerable<Seat> seats = mapper.Map<IEnumerable<Seat>>(request);

        using (var transation = context.Database.BeginTransaction())
        {
            int offsetId = context.Seat.Max(s => s.SeatId) + 1;
            foreach (var item in seats)
            {
                item.SeatId = offsetId++;
            }
            await context.Seat.AddRangeAsync(seats);
            await context.SaveChangesAsync();
            await transation.CommitAsync();
        }

        return Ok(new RangeResponse<RSeat>
        {
            Data = mapper.Map<IEnumerable<RSeat>>(seats),
            Message = "Insert seats successfully"
        });
    }
    [HttpPut]
    public async Task<IActionResult> Update([FromBody][Required][MaxLength(128)] IEnumerable<USeat> request)
    {
        IEnumerable<Seat> seats = mapper.Map<IEnumerable<Seat>>(request);
        context.Seat.UpdateRange(seats);
        await context.SaveChangesAsync();
        return Ok(new RangeResponse<RSeat>
        {
            Data = mapper.Map<IEnumerable<RSeat>>(seats),
            Message = "Update seats successfully"
        });
    }
    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody][Required][MaxLength(128)] IEnumerable<int> seatIds)
    {
        await context.Seat.Where(s => seatIds.Contains(s.SeatId)).ExecuteDeleteAsync();
        int success = await context.SaveChangesAsync();
        return Ok(new SingleResponse<int>
        {
            Data = success,
            Message = "Delete seats successfully"
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

public class CSeat
{
    [Required]
    public int Price { get; set; }
    [Required]
    [MaxLength(128)]
    public string Name { get; set; }
    [Required]
    public int BusId { get; set; }
}
public class USeat : CSeat
{
    [Required]
    public int SeatId { get; set; }
}