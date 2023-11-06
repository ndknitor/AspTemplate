static public class LinqExtensions
{
    public static IEnumerable<T> BigSkip<T>(this IEnumerable<T> items, long howMany)
        => BigSkip(items, Int32.MaxValue, howMany);

    private static IEnumerable<T> BigSkip<T>(this IEnumerable<T> items, int segmentSize, long howMany)
    {
        long segmentCount = Math.DivRem(howMany, segmentSize,
            out long remainder);

        for (long i = 0; i < segmentCount; i += 1)
            items = items.Skip(segmentSize);

        if (remainder != 0)
            items = items.Skip((int)remainder);

        return items;
    }

}