public class CursorPagingResponse<T> : RangeResponse<T>
{
    public int? Total { get; set; }
    public ulong? Cursor { get; set; }
}