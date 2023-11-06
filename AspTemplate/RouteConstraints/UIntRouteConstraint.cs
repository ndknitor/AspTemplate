using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

public class UIntRouteConstraint : IRouteConstraint
{
    public bool Match(
        HttpContext httpContext,
        IRouter route,
        string routeKey,
        RouteValueDictionary values,
        RouteDirection routeDirection)
    {
        if (values.TryGetValue(routeKey, out var valueObj) && valueObj is string valueStr)
        {
            if (uint.TryParse(valueStr, out var result))
            {
                return true;
            }
        }
        return false;
    }
}
