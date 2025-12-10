using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using EdaOdev5.Models;

namespace EdaOdev5.Filters;

/// <summary>
/// API genelinde oluþan beklenmeyen hatalarý yakalayan Exception Filter
/// Ýstemciye standart JSON hata modeli döndürür
/// </summary>
public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;
    private readonly IHostEnvironment _hostEnvironment;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger, IHostEnvironment hostEnvironment)
    {
        _logger = logger;
        _hostEnvironment = hostEnvironment;
    }

    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        var traceId = context.HttpContext.TraceIdentifier;

        // Hatayý logla
        _logger.LogError(exception,
            "?? HATA YAKALANDI | TraceId: {TraceId} | Tip: {ExceptionType} | Mesaj: {Message}",
            traceId,
            exception.GetType().Name,
            exception.Message);

        // Standart hata yanýtý oluþtur
        var errorResponse = new ErrorResponse
        {
            StatusCode = GetStatusCode(exception),
            Message = GetUserFriendlyMessage(exception),
            TraceId = traceId,
            Timestamp = DateTime.UtcNow
        };

        // Development ortamýnda detaylý hata bilgisi göster
        if (_hostEnvironment.IsDevelopment())
        {
            errorResponse.Details = exception.ToString();
        }

        context.Result = new ObjectResult(errorResponse)
        {
            StatusCode = errorResponse.StatusCode
        };

        // Hatanýn iþlendiðini belirt
        context.ExceptionHandled = true;
    }

    /// <summary>
    /// Exception tipine göre HTTP status code belirler
    /// </summary>
    private static int GetStatusCode(Exception exception) => exception switch
    {
        ArgumentNullException => StatusCodes.Status400BadRequest,
        ArgumentException => StatusCodes.Status400BadRequest,
        InvalidOperationException => StatusCodes.Status400BadRequest,
        KeyNotFoundException => StatusCodes.Status404NotFound,
        UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
        NotImplementedException => StatusCodes.Status501NotImplemented,
        _ => StatusCodes.Status500InternalServerError
    };

    /// <summary>
    /// Kullanýcý dostu hata mesajý üretir
    /// </summary>
    private static string GetUserFriendlyMessage(Exception exception) => exception switch
    {
        ArgumentNullException => "Gerekli bir parametre eksik.",
        ArgumentException => "Geçersiz parametre deðeri.",
        InvalidOperationException => "Ýþlem gerçekleþtirilemedi.",
        KeyNotFoundException => "Ýstenen kaynak bulunamadý.",
        UnauthorizedAccessException => "Bu iþlem için yetkiniz yok.",
        NotImplementedException => "Bu özellik henüz uygulanmadý.",
        _ => "Beklenmeyen bir hata oluþtu. Lütfen daha sonra tekrar deneyiniz."
    };
}

/// <summary>
/// Async versiyonu - IAsyncExceptionFilter implementasyonu
/// </summary>
public class GlobalExceptionAsyncFilter : IAsyncExceptionFilter
{
    private readonly ILogger<GlobalExceptionAsyncFilter> _logger;
    private readonly IHostEnvironment _hostEnvironment;

    public GlobalExceptionAsyncFilter(ILogger<GlobalExceptionAsyncFilter> logger, IHostEnvironment hostEnvironment)
    {
        _logger = logger;
        _hostEnvironment = hostEnvironment;
    }

    public Task OnExceptionAsync(ExceptionContext context)
    {
        var exception = context.Exception;
        var traceId = context.HttpContext.TraceIdentifier;

        _logger.LogError(exception,
            "?? ASYNC HATA YAKALANDI | TraceId: {TraceId} | Tip: {ExceptionType}",
            traceId,
            exception.GetType().Name);

        var errorResponse = new ErrorResponse
        {
            StatusCode = StatusCodes.Status500InternalServerError,
            Message = "Beklenmeyen bir hata oluþtu.",
            TraceId = traceId,
            Timestamp = DateTime.UtcNow,
            Details = _hostEnvironment.IsDevelopment() ? exception.Message : null
        };

        context.Result = new ObjectResult(errorResponse)
        {
            StatusCode = errorResponse.StatusCode
        };

        context.ExceptionHandled = true;

        return Task.CompletedTask;
    }
}
