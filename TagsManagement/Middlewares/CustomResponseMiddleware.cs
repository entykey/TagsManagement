// add this line in Program:    app.UseMiddleware<CustomResponseMiddleware>();
namespace TagsManagement.Middlewares
{
    using System.Text.Json;
    using TagsManagement.DomainModels;


    public class CustomResponseMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;

            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                context.Response.StatusCode = StatusCodes.Status200OK;

                var message = "Unauthorized client";

                var result = new CustomResponseModel
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = message
                };

                var json = JsonSerializer.Serialize(result);
                await context.Response.WriteAsync(json);
            }
            else
            {
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                await context.Response.Body.CopyToAsync(originalBodyStream);
            }
        }
    }

}
