using System.Linq.Expressions;

public static class ArrayExtensions
{
    public static void MapProperties<TSource, TResult>(
        this IEnumerable<TSource> targetArray,
        Expression<Func<TSource, TResult>> propertyExpression,
        IEnumerable<TResult> sourceArray)
    {
        if (targetArray == null)
        {
            throw new ArgumentNullException(nameof(targetArray));
        }

        if (propertyExpression == null)
        {
            throw new ArgumentNullException(nameof(propertyExpression));
        }

        if (sourceArray == null)
        {
            throw new ArgumentNullException(nameof(sourceArray));
        }

        if (targetArray.Count() != sourceArray.Count())
        {
            throw new ArgumentException("Source and target arrays must have the same length.");
        }

        var memberExpression = (MemberExpression)propertyExpression.Body;
        var propertyName = memberExpression.Member.Name;

        for (int i = 0; i < targetArray.Count(); i++)
        {
            typeof(TSource).GetProperty(propertyName)?.SetValue(targetArray.ElementAt(i), sourceArray.ElementAt(i));
        }
    }
}