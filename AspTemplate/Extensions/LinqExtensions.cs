static public class LinqExtensions
{
    public static IEnumerable<T> BigSkip<T>(this IEnumerable<T> items, long howMany)
    {
        long segmentCount = Math.DivRem(howMany, int.MaxValue, out long remainder);

        for (long i = 0; i < segmentCount; i += 1)
            items = items.Skip(int.MaxValue);

        if (remainder != 0)
            items = items.Skip((int)remainder);

        return items;
    }
}