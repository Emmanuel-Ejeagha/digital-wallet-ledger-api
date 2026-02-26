namespace DigitalWallet.API.Middleware;
/// <summary>
/// Middleware that logs every request and its duration
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await _next(context);
        }
        finally 
        {
            stopwatch.Stop();
            var user = context.User.Identity?.Name ?? "anonymous";
            _logger.LogInformation(
                "Http {Method} {Path} responded {StatusCode} in {ElapsedMs}ms (User: {User})",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                user);
        }
    }
}
