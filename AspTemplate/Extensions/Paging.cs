public static class Paging
{
    public static int GetMaxPage(int limit, int total)
    {
        if (total == 0)
        {
            return 0;
        }
        return (int)Math.Ceiling(((double)total / (double)limit));
    }
    public static int GetOffset(int page, int limit)
    {
        if (page <= 0)
        {
            page = 1;
        }
        return ((page - 1) * limit);
    }
    public static int GetOffset(PagingRequest request)
    {
        return (request.Page - 1) * request.Size;
    }
}