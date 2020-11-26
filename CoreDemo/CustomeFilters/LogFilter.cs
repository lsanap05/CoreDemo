using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CoreDemo.CustomFilters
{
    public class LogFilter : ActionFilterAttribute, IActionFilter
    {
        private void LogRequest(string status, RouteData routeData) 
        {
            if (routeData.Routers.Count > 0)
            {
                string controllerName = routeData.Values["controller"].ToString();
                string actionName = routeData.Values["action"].ToString();


                Debug.WriteLine($"Current State of Request is {status} " +
                    $"in controller {controllerName} " +
                    $"in action {actionName}");
            }
            
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            LogRequest("Action Executing", context.RouteData);
        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            LogRequest("Action Executed", context.RouteData);
        }
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            LogRequest("Result Executing", context.RouteData);
        }
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            LogRequest("Result Executed", context.RouteData);
        }
    }
}
