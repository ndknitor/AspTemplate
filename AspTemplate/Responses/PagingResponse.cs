public class PagingResponse<T> : RangeResponse<T>
{
    public required int Size;
    public required int Total { get; set; }
    public int MaxPage { get { return (int)Math.Ceiling(((double)Total / (double)Size)); } }
}