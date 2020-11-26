using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoreDemo.CustoMiddlewares
{
    /// <summary>
    ///  Clas that will be used to structure the error message
    /// </summary>
    public class ErrorInformation
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public string UsarName { get; set; }
        public string Modelstate { get; set; }
        
    }

    /// <summary>
    /// The custome middleware class
    /// </summary>
    public class ExceptionMiddleware: ExceptionFilterAttribute
    {
        private readonly RequestDelegate request;
        private readonly IModelMetadataProvider modelMetadata;

        public ExceptionMiddleware(RequestDelegate request, IModelMetadataProvider modelMetadata)
        {
            this.request = request;
            this.modelMetadata = modelMetadata;
        }

        /// <summary>
        /// The method that will contains logic for custom middlewares
        /// This method will be invoked by HttpContext
        /// when registered in Request Pipeline
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // if no exception occures during request processing then
                // go to next middleware in Request Pipeline
                await request(context);
            }
            catch (Exception ex)
            {
                // logic to handle exception
                // set the response statuc code
                context.Response.StatusCode = 500;
                // read the error message
                string message = ex.Message;
                //// context.Request.Headers["Authorization"]
                var UsarName = ((ClaimsIdentity)((DefaultHttpContext)context).User.Identity).Name;
                //var modelstate = exceptionContext.ModelState.ToString();
                //var dictionary = new ViewDataDictionary(modelMetadata, exceptionContext.ModelState);
                // structure the error message
                var errorInfo = new ErrorInformation()
                {
                    ErrorCode = context.Response.StatusCode,
                    ErrorMessage = message,
                    UsarName = UsarName,
                  //  Modelstate=modelstate
                };
                // serialize the message in JSON format
                var responseError = JsonConvert.SerializeObject(errorInfo);
                // write the response for the request
                await context.Response.WriteAsync(responseError);
            }
        }
    }

    /// <summary>
    /// The class that isused to register the custom middleware
    /// </summary>
    public static class CustomMiddleware
    {
        /// <summary>
        /// This will be an extension method to the
        /// IApplicationBuilder interface
        /// </summary>
        public static void UseCustomeException(this IApplicationBuilder builder)
          {
            builder.UseMiddleware<ExceptionMiddleware>();
        }
    }

}
