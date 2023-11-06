using System.Text.Json.Serialization;

public class PagingResponse<T> : RangeResponse<T>
{
    [JsonIgnore]
    public int Size { get; set; }
    public int Total { get; set; }
    public int MaxPage { get { return (int)Math.Ceiling(((double)Total / (double)Size)); } }
}