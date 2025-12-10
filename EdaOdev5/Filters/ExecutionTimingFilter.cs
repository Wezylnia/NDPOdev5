using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace EdaOdev5.Filters;

/// <summary>
/// Action çalýþma süresini ölçen ve loglayan filter
/// OnActionExecuting: Süreyi baþlatýr
/// OnActionExecuted: Süreyi durdurur ve loglar
/// </summary>
public class ExecutionTimingFilter : IActionFilter
{
    private readonly ILogger<ExecutionTimingFilter> _logger;
    private const string StopwatchKey = "ActionStopwatch";

    public ExecutionTimingFilter(ILogger<ExecutionTimingFilter> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Action çalýþmaya baþlamadan önce çaðrýlýr
    /// </summary>
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        context.HttpContext.Items[StopwatchKey] = stopwatch;

        var controllerName = context.RouteData.Values["controller"]?.ToString();
        var actionName = context.RouteData.Values["action"]?.ToString();

        _logger.LogInformation(
            "?? ACTION BAÞLADI | Controller: {Controller} | Action: {Action} | Zaman: {Time}",
            controllerName,
            actionName,
            DateTime.UtcNow.ToString("HH:mm:ss.fff"));
    }

    /// <summary>
    /// Action çalýþtýktan sonra çaðrýlýr
    /// </summary>
    public void OnActionExecuted(ActionExecutedContext context)
    {
        var controllerName = context.RouteData.Values["controller"]?.ToString();
        var actionName = context.RouteData.Values["action"]?.ToString();

        if (context.HttpContext.Items.TryGetValue(StopwatchKey, out var value) && value is Stopwatch stopwatch)
        {
            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            var statusIcon = context.Exception == null ? "?" : "?";
            
            _logger.LogInformation(
                "{Icon} ACTION BÝTTÝ | Controller: {Controller} | Action: {Action} | Süre: {Duration}ms",
                statusIcon,
                controllerName,
                actionName,
                elapsedMs);
        }
    }
}

/// <summary>
/// Async versiyonu - IAsyncActionFilter implementasyonu
/// </summary>
public class ExecutionTimingAsyncFilter : IAsyncActionFilter
{
    private readonly ILogger<ExecutionTimingAsyncFilter> _logger;

    public ExecutionTimingAsyncFilter(ILogger<ExecutionTimingAsyncFilter> logger)
    {
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var controllerName = context.RouteData.Values["controller"]?.ToString();
        var actionName = context.RouteData.Values["action"]?.ToString();

        _logger.LogInformation(
            "?? ACTION BAÞLADI | Controller: {Controller} | Action: {Action}",
            controllerName,
            actionName);

        var stopwatch = Stopwatch.StartNew();

        var executedContext = await next();

        stopwatch.Stop();

        var statusIcon = executedContext.Exception == null ? "?" : "?";
        
        _logger.LogInformation(
            "{Icon} ACTION BÝTTÝ | Controller: {Controller} | Action: {Action} | Süre: {Duration}ms",
            statusIcon,
            controllerName,
            actionName,
            stopwatch.ElapsedMilliseconds);
    }
}
