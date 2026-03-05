namespace ECommerce.core.Middlewares;


public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // X-Content-Type-Options: Prevents browsers from MIME sniffing
        // Forces the browser to respect the Content-Type header
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";

        // X-Frame-Options: Prevents clickjacking attacks
        // DENY ensures the page cannot be displayed in a frame, iframe, embed or object
        context.Response.Headers["X-Frame-Options"] = "DENY";

        // Referrer-Policy: Controls how much referrer information is shared
        // strict-origin-when-cross-origin sends full URL for same-origin, only origin for cross-origin
        context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

        // Content-Security-Policy: Prevents script injection and other content injection attacks
        // default-src 'self' - Only allow resources from same origin by default
        // script-src 'self' 'unsafe-inline' - Allow scripts from same origin (unsafe-inline for development/swagger)
        // style-src 'self' 'unsafe-inline' - Allow styles from same origin
        // img-src 'self' data: https: - Allow images from same origin, data URIs, and HTTPS
        // font-src 'self' - Allow fonts from same origin
        // connect-src 'self' - Allow connections (XMLHttpRequest, WebSocket) to same origin
        // frame-ancestors 'none' - No one can frame this page
        context.Response.Headers["Content-Security-Policy"] = "default-src 'self'; script-src 'self' 'unsafe-inline' 'unsafe-eval'; style-src 'self' 'unsafe-inline'; img-src 'self' data: https:; font-src 'self'; connect-src 'self'; frame-ancestors 'none'";

        // X-Permitted-Cross-Domain-Policies: Restricts cross-domain policy
        context.Response.Headers["X-Permitted-Cross-Domain-Policies"] = "none";

        // X-XSS-Protection: Legacy XSS protection header (mainly for older browsers)
        // 1; mode=block enables XSS protection and blocks page if attack detected
        context.Response.Headers["X-XSS-Protection"] = "1; mode=block";

        // Permissions-Policy: Controls which browser features and APIs can be used
        context.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";

        await _next(context);
    }
}
