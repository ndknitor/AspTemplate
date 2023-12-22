using System.ComponentModel.DataAnnotations;

public class OffsetPagingRequest
{
    [MaxLength(48)]
    public IEnumerable<bool> Desc { get; set; }
    [MaxLength(48)]
    public virtual IEnumerable<string> OrderBy { get; set; }
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;
    [Range(1, 128)]
    public int Size { get; set; } = 64;
    [SwaggerExclude]
    public int Offset { get => (Page - 1) * Size; }
}