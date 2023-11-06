public static class GuidUtility
{
    public static IEnumerable<Guid> GetGuids(int count)
    {
        if (count <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than zero.");
        }

        var uniqueGuids = new HashSet<Guid>();

        while (uniqueGuids.Count < count)
        {
            var newGuid = Guid.NewGuid();

            if (uniqueGuids.Add(newGuid))
            {
                yield return newGuid;
            }
        }
    }
    public static IEnumerable<string> GetGuidStrings(int count)
    {
        return GetGuids(count).Select(g => g.ToString());
    }
}