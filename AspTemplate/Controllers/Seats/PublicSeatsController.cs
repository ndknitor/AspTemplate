using System.ComponentModel.DataAnnotations;
using AspTemplate.Context;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace NewTemplate.Controllers;
[ApiController]
[Route("/api/public/seats")]
public class PublicSeatsController(EtdbContext context, IMapper mapper) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Tags = [nameof(Seat)])]
    public async Task<IActionResult> Get([FromQuery] OffsetPagingRequest request)
    {
        var total = await context.Seat.Where(s => !s.Deleted).CountAsync();
        var seats = context.Seat
                            .Where(s => !s.Deleted)
                            .OrderBy(s => s.SeatId)
                            .Skip(request.Offset)
                            .Take(request.Size)
                            .Select(mapper.Map<RSeat>);
        return Ok(new OffsetPagingResponse<RSeat>
        {
            Size = request.Size,
            Data = seats,
            Message = $"There are {total} seats avalible",
            Total = total
        });
    }
    [HttpGet("range")]
    [SwaggerOperation(Tags = [nameof(Seat)])]
    public IActionResult GetByIds([FromQuery] IEnumerable<int> seatId, [FromQuery] IEnumerable<string> name)
    {
        return Ok(new RangeResponse<RSeat>
        {
            Data = mapper.Map<IEnumerable<RSeat>>(GetRange(context, seatId, name)),
            Message = "Get seat range"
        });
    }
    [HttpGet("error")]
    [SwaggerOperation(Tags = [nameof(Seat)])]
    public IActionResult GetError([FromQuery] IEnumerable<int> seatIds)
    {
        int a = int.Parse("a");
        return Ok(new RangeResponse<Seat>
        {
            Data = null,
            Message = "Get seats successfully"
        });
    }
    private static readonly Func<EtdbContext, IEnumerable<int>, IEnumerable<string>, IEnumerable<Seat>> GetRange = EF.CompileQuery
    (
        (EtdbContext context, IEnumerable<int> seatIds, IEnumerable<string> names) =>
        context.Seat
        .Where(s => seatIds.Contains(s.SeatId) || names.Contains(s.Name))
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