using System.Diagnostics;

namespace EdaOdev5.Middleware;

/// <summary>
/// Her gelen isteði ve dönen yanýtý loglayan custom middleware
/// Request: HTTP metod, URL yolu ve zaman
/// Response: Status Code bilgisi
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
        // Ýstek baþlangýç zamaný
        var startTime = DateTime.UtcNow;
        var stopwatch = Stopwatch.StartNew();

        // REQUEST bilgilerini logla
        var requestMethod = context.Request.Method;
        var requestPath = context.Request.Path;
        var queryString = context.Request.QueryString;
        
        _logger.LogInformation(
            "???????????????????????????????????????????????????????????????????????");
        _logger.LogInformation(
            "?? REQUEST | Zaman: {Time} | Metod: {Method} | URL: {Path}{Query}",
            startTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            requestMethod,
            requestPath,
            queryString);

        try
        {
            // Sonraki middleware'e geç
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            
            // RESPONSE bilgilerini logla
            var statusCode = context.Response.StatusCode;
            var statusText = GetStatusText(statusCode);
            
            _logger.LogInformation(
                "?? RESPONSE | Status: {StatusCode} ({StatusText}) | Süre: {Duration}ms",
                statusCode,
                statusText,
                stopwatch.ElapsedMilliseconds);
            _logger.LogInformation(
                "???????????????????????????????????????????????????????????????????????\n");
        }
    }

    private static string GetStatusText(int statusCode) => statusCode switch
    {
        200 => "OK",
        201 => "Created",
        204 => "No Content",
        400 => "Bad Request",
        401 => "Unauthorized",
        403 => "Forbidden",
        404 => "Not Found",
        500 => "Internal Server Error",
        _ => "Unknown"
    };
}

/// <summary>
/// Middleware'i pipeline'a eklemek için extension method
/// </summary>
public static class RequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}
