using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using StackExchange.Redis;

public class RedisSession : ISession
{
    private readonly HttpContext context;
    private readonly IDatabase redis;
    public RedisSession(HttpContextAccessor accessor, IDatabase redis)
    {
        context = accessor.HttpContext;
        this.redis = redis;
    }
    public bool IsAvailable => true;

    public string Id => $"{nameof(RedisSession)}{context.User.FindFirstValue(ClaimTypes.NameIdentifier)}";

    public IEnumerable<string> Keys => throw new NotImplementedException();

    public void Clear()
    {
        throw new NotImplementedException();
    }

    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task LoadAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Remove(string key)
    {
        throw new NotImplementedException();
    }

    public void Set(string key, byte[] value)
    {
        
    }

    public bool TryGetValue(string key, [NotNullWhen(true)] out byte[] value)
    {
        throw new NotImplementedException();
    }
}