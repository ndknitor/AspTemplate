using System.ComponentModel.DataAnnotations;

public class PagingRequest
{
    [MaxLength(48)]
    public IEnumerable<bool> Desc { get; set; }
    [MaxLength(48)]
    public virtual IEnumerable<string> OrderBy { get; set; }
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;
    [Range(1, 256)]
    public int Size { get; set; } = 50;
    [SwaggerExclude]
    public int Offset { get { return (Page - 1) * Size; } }
}