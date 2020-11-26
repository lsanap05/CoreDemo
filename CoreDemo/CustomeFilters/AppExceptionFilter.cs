
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Core_WebApp.CustomFilters
{
    public class AppExceptionFilter : ExceptionFilterAttribute
    {
        private readonly IModelMetadataProvider modelMetadata;
        /// <summary>
        /// Read the Model metadata used in request Procesing
        /// </summary>
        /// <param name="modelMetadata"></param>
        public AppExceptionFilter(IModelMetadataProvider modelMetadata)
        {
            this.modelMetadata = modelMetadata;
            
        }
        /// <summary>
        /// Method that will define the custom error page result
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(ExceptionContext context)
        {
            Exception ex = context.Exception;
          

            // Handle Exception to complete the process
            context.ExceptionHandled = true;
            // read the error message
            string message = context.Exception.Message;
            // define the resut
            var viewResult = new ViewResult() { ViewName = "CustomError" };
            // define ViewDataDictionary to pass data to ViewResult
            var dictionary = new ViewDataDictionary(modelMetadata, context.ModelState);
            dictionary["RequestId"] = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
            // define keys for the dictioary
            if (context.RouteData.Routers.Count > 0)
            {
                dictionary["ControllerName"] = context.RouteData.Values["controller"].ToString();
                dictionary["ActionName"] = context.RouteData.Values["action"].ToString();
            }
            dictionary["ErrorMessage"] = message;
            dictionary["DateTime"] = DateTime.Now.ToString();
            viewResult.ViewData = dictionary;
            // define the result
            context.Result = viewResult;
        }
        //public async Task Invoke(HttpContext context)
        //{
        //    try
        //    {
        //        await _next.Invoke(context);
        //        if (context.Response.StatusCode == StatusCodes.Status404NotFound)
        //        {
        //            var statusCodeFeature = context.Features.Get<IStatusCodePagesFeature>();
        //            if (statusCodeFeature == null || !statusCodeFeature.Enabled)
        //            {
        //                // there's no StatusCodePagesMiddleware in app
        //                if (!context.Response.HasStarted)
        //                {
        //                    var view = new ErrorPage(new ErrorPageModel());
        //                    await view.ExecuteAsync(context);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // TODO: stay tuned
        //        throw;
        //    }
        //}
    }
}
