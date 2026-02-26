using System.Net;
using System.Text.Json;
using ECommerce.core.Exceptions;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode status;
        string message = exception.Message;

        switch (exception)
        {
            case UnauthorizedAccessException:
                status = HttpStatusCode.Unauthorized;
                break;
            case ArgumentException:
                status = HttpStatusCode.BadRequest;
                break;
            case BadRequestException:
                status = HttpStatusCode.BadRequest;
                break;
            default:
                status = HttpStatusCode.InternalServerError;
                message = "An unexpected error occurred.";
                break;
        }

        var response = new { message };
        var payload = JsonSerializer.Serialize(response);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;

        return context.Response.WriteAsync(payload);
    }
}