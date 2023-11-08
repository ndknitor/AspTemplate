using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
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

        // Attempt to retrieve the result from the cache
        var cachedResult = await cache.StringGetAsync(cacheKey);

        if (!cachedResult.HasValue)
        {
            // If the result is not in the cache, proceed with the action
            var resultContext = await next();

            // Check if the action's result is an ObjectResult and serialize it to cache
            if (resultContext.Result is Microsoft.AspNetCore.Mvc.ObjectResult objectResult)
            {
                var resultContent = Newtonsoft.Json.JsonConvert.SerializeObject(objectResult.Value);
                // Store the result in the cache
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