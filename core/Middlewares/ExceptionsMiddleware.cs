using ECommerce.core.Exceptions;
using ECommerce.DTOs;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UnauthorizedException ex)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(
                ApiResponse.ErrorResponse(ex.Message)
            );
        }
        catch (BadRequestException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(
                ApiResponse.ErrorResponse(ex.Message)
            );
        }
        // catch (NotFoundException ex)
        // {
        //     context.Response.StatusCode = StatusCodes.Status404NotFound;
        //     await context.Response.WriteAsJsonAsync(
        //         ApiResponse.ErrorResponse(ex.Message)
        //     );
        // }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(
                ApiResponse.ErrorResponse("Internal Server Error")
            );
        }
    }
}