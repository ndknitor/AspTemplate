public class LoggingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger logger;
    public LoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
    {
        this.next = next;
        logger = loggerFactory.CreateLogger(nameof(LoggingMiddleware));
    }
    public async Task Invoke(HttpContext context)
    {
        bool buffering = context.Request.ContentType != null && context.Request.ContentType.Contains(System.Net.Mime.MediaTypeNames.Application.Json);
        string ip = context.Connection.RemoteIpAddress.ToString(); //context.Request.Headers["CF-Connecting-IP"].FirstOrDefault();
        if (buffering)
        {
            context.Request.EnableBuffering();
        }
        try
        {
            logger.LogInformation($"Authorization: {context.Request.Headers["Authorization"]}");
            await next(context);
            var responseString =
@$"
[HTTP RESPONSE]
💳 Connection Id: {context.Connection.Id}
👤 Client IP: {ip}
🕵️ User-Agent: {context.Request.Headers["User-Agent"].FirstOrDefault()}
🛣️ Path: {context.Request.Path}
🤖 Method: {context.Request.Method}
🔍 Query: {context.Request.QueryString}
🔢 Status Code: {context.Response.StatusCode}
";
            int statusCode = context.Response.StatusCode;
            if (statusCode < 400)
            {
                logger.LogInformation(responseString);
            }
            else if (statusCode >= 400 && statusCode <= 500)
            {
                logger.LogWarning(responseString);
            }
        }
        catch (System.Exception e)
        {
            var requestBody = "Not a JSON request";
            if (buffering)
            {
                using (var reader = new StreamReader(context.Request.Body, encoding: System.Text.Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 4096, leaveOpen: true))
                {
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);
                    requestBody = await reader.ReadToEndAsync();
                }
            }
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.Body.Close();
            var responseString =
@$"
[HTTP RESPONSE ERROR]
💳 Connection Id : {context.Connection.Id}
👤 Client IP: {ip}
🛣️ Path: {context.Request.Path}
🤖 Method: {context.Request.Method}
🔍 Query: {context.Request.QueryString}
🔢 Status Code: {context.Response.StatusCode}
❗ Error: {e.Message}
🔴 Request body: {requestBody}
";
            logger.LogError(responseString);
        }

    }
}