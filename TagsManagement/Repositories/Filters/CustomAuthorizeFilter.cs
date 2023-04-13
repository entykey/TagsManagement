using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace TagsManagement.Repositories.Filters
{
    public class CustomAuthorizeFilter : IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new ContentResult
                {
                    StatusCode = 401,
                    ContentType = "text/html",
                    Content = "<HTML><HEAD><TITLE>Access Denied</TITLE></HEAD><BODY><H1>Access Denied</H1>You don't have permission to access this resource.</BODY></HTML>"
                };
            }
            else
            {
                // Check if the user is authorized to access the resource
                // ...

                //if (/* user is not authorized */)
                //{
                    context.Result = new ContentResult
                    {
                        StatusCode = 403,
                        ContentType = "text/html",
                        Content = "<HTML><HEAD><TITLE>Access Denied</TITLE></HEAD><BODY><H1>Access Denied</H1>You don't have permission to access this resource.</BODY></HTML>"
                    };
                //}
            }
        }
    }
}
