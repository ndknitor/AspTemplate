using System.Security.Claims;

public static class ClaimsPrincipalExtension
{
    public static T UserId<T>(this ClaimsPrincipal principal, string key = ClaimTypes.NameIdentifier)
    where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
        if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
        {
            throw new ArgumentException("Floating-point types are not supported as TKey.");
        }
        return (T)Convert.ChangeType(principal.FindFirstValue(key), typeof(T));
    }
}