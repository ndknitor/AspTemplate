using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

public static class DbContextExtensions
{
    public static TKey GetLastInsertedId<TEntity, TKey>(this DbContext context)
        where TEntity : class
        where TKey : struct, IComparable, IComparable<TKey>, IConvertible, IEquatable<TKey>, IFormattable
    {
        if (typeof(TKey) == typeof(float) || typeof(TKey) == typeof(double))
        {
            throw new ArgumentException("Floating-point types are not supported as TKey.");
        }
        TKey maxTKey = default;
        if (typeof(TKey) == typeof(int))
        {
            maxTKey = (TKey)(object)int.MaxValue;
        }
        else if (typeof(TKey) == typeof(uint))
        {
            maxTKey = (TKey)(object)uint.MaxValue;
        }
        else if (typeof(TKey) == typeof(long))
        {
            maxTKey = (TKey)(object)long.MaxValue;
        }
        else if (typeof(TKey) == typeof(ulong))
        {
            maxTKey = (TKey)(object)ulong.MaxValue;
        }

        var entityType = context.Model.FindEntityType(typeof(TEntity));
        var primaryKey = entityType.FindPrimaryKey();
        var primaryKeyPropertyName = primaryKey.Properties.First().Name;

        var relationalEntityType = entityType.GetAnnotations()
            .SingleOrDefault(a => a.Name == "Relational:TableName");

        if (relationalEntityType == null)
        {
            throw new InvalidOperationException("Table name annotation not found.");
        }

        var tableName = relationalEntityType.Value?.ToString();

        var sql = $"SELECT MAX([{primaryKeyPropertyName}]) FROM [{tableName}] where [{primaryKeyPropertyName}] <= {Environment.GetEnvironmentVariable("ID_ONSET") ?? Convert.ChangeType(maxTKey, typeof(string))}";

        var dbConnection = context.Database.GetDbConnection();
        if (dbConnection.State != System.Data.ConnectionState.Open)
        {
            dbConnection.Open();
        }

        using var command = dbConnection.CreateCommand();
        command.CommandText = sql;
        object result = command.ExecuteScalar();
        var lastInsertedId = result == DBNull.Value ? (TKey)Convert.ChangeType(Environment.GetEnvironmentVariable("ID_OFFSET") ?? "0", typeof(TKey)) : (TKey)Convert.ChangeType(result, typeof(TKey));

        return lastInsertedId;
    }
    public static TKey GetInsertId<TEntity, TKey>(this DbContext context)
    where TEntity : class
    where TKey : struct, IComparable, IComparable<TKey>, IConvertible, IEquatable<TKey>, IFormattable
    {
        return ((dynamic)GetLastInsertedId<TEntity, TKey>(context)) + 1;
    }
    public static IEnumerable<TEntity> GetInsertEntities<TEntity, TKey>(this DbContext context, IEnumerable<TEntity> entities)
    where TEntity : class
    where TKey : struct, IComparable, IComparable<TKey>, IConvertible, IEquatable<TKey>, IFormattable
    {
        dynamic id = GetInsertId<TEntity, TKey>(context);
        foreach (var item in entities)
        {
            yield return GetInsertEntity<TEntity, TKey>(context, item, id);
            id++;
        }
    }
    public static TEntity GetInsertEntity<TEntity, TKey>(this DbContext context, TEntity entity)
    where TEntity : class
    where TKey : struct, IComparable, IComparable<TKey>, IConvertible, IEquatable<TKey>, IFormattable
    {
        return GetInsertEntity<TEntity, TKey>(context, entity, GetInsertId<TEntity, TKey>(context));
    }
    private static TEntity GetInsertEntity<TEntity, TKey>(this DbContext context, TEntity entity, TKey id)
    where TEntity : class
    where TKey : struct, IComparable, IComparable<TKey>, IConvertible, IEquatable<TKey>, IFormattable
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        // Get the primary key property of the entity
        IKey primaryKey = context.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey();
        if (primaryKey == null)
        {
            throw new InvalidOperationException($"Entity {typeof(TEntity).Name} does not have a primary key.");
        }

        PropertyInfo idProperty = entity.GetType().GetProperty(primaryKey.Properties[0].Name);
        if (idProperty == null)
        {
            throw new InvalidOperationException($"Entity {typeof(TEntity).Name} does not have a primary key property.");
        }

        // Load the ID property with a default value
        object idValue = id;
        idProperty.SetValue(entity, idValue);
        return entity;
    }
}