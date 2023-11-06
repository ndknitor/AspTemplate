using NewTemplate.Context;

public class GetSeatRequest : PagingRequest
{
    [ClassProperty(typeof(Seat), nameof(Seat.Price))]
    public override IEnumerable<string> OrderBy { get => base.OrderBy; set => base.OrderBy = value; }
}