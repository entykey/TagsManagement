// DESC:  implement single API key auth middleware for an authorized client
// SOURCE: https://www.c-sharpcorner.com/article/using-api-key-authentication-to-secure-asp-net-core-web-api/
// add this line in Program:    app.UseMiddleware<ApiKeyMiddleware>();
// add the API key in appsettings:      "XApiKey": "pgH7QzFHJx4w46fI~5Uzi4RvtTwlEXp"

namespace TagsManagement.Middlewares
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string APIKEY = "XApiKey";
        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // Note: InvokeAsync method is defined in this middleware
        // so that it will contain the main process, in our case,
        // the main process will be to search and validate the ApiKey header
        // name and value within the httpcontext request headers collection.
        public async Task InvokeAsync(HttpContext context)
        {
            #region check if ApiKey is provided
            // debug way (show if apikey missing):
            //if (!context.Request.Headers.TryGetValue(APIKEY, out
            //        var extractedApiKey))
            //{
            //    context.Response.StatusCode = 401;
            //    await context.Response.WriteAsync("Api Key was not provided");
            //    return;
            //}

            // secure way:
            if (!context.Request.Headers.TryGetValue(APIKEY, out
                    var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized client");
                return;
            }
            #endregion

            #region check if ApiKey is valid
            var appSettings = context.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = appSettings.GetValue<string>(APIKEY);
            if (!apiKey.Equals(extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized client");
                return;
            }
            await _next(context);
            #endregion
        }
    }
}
