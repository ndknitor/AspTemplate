using Microsoft.AspNetCore.Mvc.Filters;
public class NonAuthorizedAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.HttpContext.User.Identity.IsAuthenticated)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            var res = new
            {
                Message = "User is authorized"
            };
            await context.HttpContext.Response.WriteAsync(res.ToJson());
        }
        else
        {
            await next();
        }
    }
}