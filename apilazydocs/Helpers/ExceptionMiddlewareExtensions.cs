using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
namespace ApiLazyDoc.Helpers
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        if (contextFeature.Error is AppException)
                        {
                            var exception = contextFeature.Error as AppException;
                            context.Response.StatusCode = exception.statusCode;
                            context.Response.ContentType = "text/html";

                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = exception.statusCode,
                                Message = exception.Message
                            }.ToString());
                        }
                        else
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            context.Response.ContentType = "text/html";
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = context.Response.StatusCode,
                                Message = "Internal Server Error."
                            }.ToString());
                        }
                    }
                });
            });
        }
    }
}
