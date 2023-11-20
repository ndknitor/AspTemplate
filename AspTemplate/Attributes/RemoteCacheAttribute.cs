using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;
using StackExchange.Redis;
public class RemoteCacheAttribute : ActionFilterAttribute
{
    private readonly int expriedMin;
    private readonly bool queryIdentity;
    public RemoteCacheAttribute(bool queryIdentity = false, int expriedMin = 8)
    {
        this.expriedMin = expriedMin;
        this.queryIdentity = queryIdentity;
    }
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var cache = context.HttpContext.RequestServices.GetRequiredService<IDatabase>();
        string cacheKey = $"RemoteCache:{context.HttpContext.Request.Path}" + (queryIdentity == true ? context.HttpContext.Request.QueryString : "");
        var cachedResult = await cache.StringGetAsync(cacheKey);

        if (!cachedResult.HasValue)
        {
            var resultContext = await next();

            if (resultContext.Result is Microsoft.AspNetCore.Mvc.ObjectResult objectResult)
            {
                var resultContent = Newtonsoft.Json.JsonConvert.SerializeObject(objectResult.Value);
                await cache.StringSetAsync(cacheKey, resultContent, TimeSpan.FromMinutes(expriedMin));
            }
        }
        else
        {
            context.HttpContext.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.Json;
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            await context.HttpContext.Response.WriteAsync(cachedResult);
        }
    }
}