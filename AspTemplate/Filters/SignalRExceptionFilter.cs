using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

public class SignalRExceptionFilter : IHubFilter
{
    private readonly ILogger<SignalRExceptionFilter> _logger;

    public SignalRExceptionFilter(ILogger<SignalRExceptionFilter> logger)
    {
        _logger = logger;
    }

    public async ValueTask<object> InvokeMethodAsync(
        HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object>> next)
    {
        try
        {
            return await next(invocationContext);
        }
        catch (Exception ex)
        {
            string ip = invocationContext.Context.GetHttpContext().Connection.RemoteIpAddress.ToString(); //invocationContext.Context.GetHttpContext().Request.Headers["CF-Connecting-IP"].FirstOrDefault();
            _logger.LogError(@$"
[SIGNALR ERROR]
💳 Connection Id: {invocationContext.Context.ConnectionId}
👤 Client IP: {invocationContext.Context.GetHttpContext().Connection.RemoteIpAddress}
🤖 Method: {invocationContext.HubMethodName}
❗ Error: {ex.Message}
🔴 Message: {JsonConvert.SerializeObject(invocationContext.HubMethodArguments)}");
            throw;
        }
    }
}