using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using TagsManagement.Middlewares.Interfaces;

// https://www.codeproject.com/Tips/5337523/Response-Time-Header-in-ASP-NET-Core
// Action Filter

// require these classes:
// Middlewares > Interface > IStopwatch.cs
// Middlewares > ResponseTimeMiddleware.cs

namespace TagsManagement.Filters
{
    [AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Method)]
    public class ResponseTimeFilter : Attribute, IActionFilter
    {
        private IActionResponseTimeStopwatch GetStopwatch(HttpContext context)
        {
            return context.RequestServices.GetService<IActionResponseTimeStopwatch>();
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            IStopwatch watch = GetStopwatch(context.HttpContext);
            watch.Reset();
            watch.Start();
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            IStopwatch watch = GetStopwatch(context.HttpContext);
            watch.Stop();
            string value = string.Format("{0}ms", watch.ElapsedMilliseconds);
            context.HttpContext.Response.Headers["X-Action-Response-Time"] = value;
        }
    }

    public interface IActionResponseTimeStopwatch : IStopwatch
    {
    }

    public class ActionResponseTimeStopwatch : Stopwatch, IActionResponseTimeStopwatch
    {
        public ActionResponseTimeStopwatch() : base()
        {
        }
    }
}
