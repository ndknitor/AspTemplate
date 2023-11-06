public class RangeResponse<T> : StandardResponse
{
    public IEnumerable<T> Data { get; set; }
}