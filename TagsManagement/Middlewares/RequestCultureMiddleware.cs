// @Source: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/write?view=aspnetcore-7.0

using TagsManagement.Middlewares;

namespace TagsManagement.Middlewares
{
    using System.Globalization;


    public class RequestCultureMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestCultureMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var cultureQuery = context.Request.Query["culture"];
            if (!string.IsNullOrWhiteSpace(cultureQuery))
            {
                var culture = new CultureInfo(cultureQuery);

                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
                context.Response.Headers["Culture"] = CultureInfo.CurrentCulture.DisplayName;
            }

            // Call the next delegate/middleware in the pipeline.
            await _next(context);
        }
    }
}

public static class RequestCultureMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestCulture(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestCultureMiddleware>();
    }
}
