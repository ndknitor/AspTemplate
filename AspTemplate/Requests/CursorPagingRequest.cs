using System.ComponentModel.DataAnnotations;

public class CursorPagingRequest
{
    [MaxLength(48)]
    public IEnumerable<bool> Desc { get; set; }
    [MaxLength(48)]
    public virtual IEnumerable<string> OrderBy { get; set; }
    public ulong? Cursor { get; set; }
    public bool CursorDesc { get; set; } = false;
    [Range(1, 128)]
    public int Size { get; set; } = 64;

}