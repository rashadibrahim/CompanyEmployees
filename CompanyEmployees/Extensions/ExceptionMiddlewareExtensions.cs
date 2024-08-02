using Contracts;
using Entities.ErrorModel;
using Entities.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace CompanyEmployees.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {

        public static void ConfigureExceptionHandler(this WebApplication app, ILoggerManager logger)
        {
//UseExceptionHandler method registers middleware that will be invoked whenever an unhandled exception occurs within the application pipeline.

            app.UseExceptionHandler(appError => // a lambda expression that defines the logic to be executed when an exception occurs.
            {
                // This method allows you to define how the response to the client should be formatted and sent when an exception occurs.
                appError.Run(async context =>
                {
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        if (contextFeature.Error is NotFoundException)
                        {
                            context.Response.StatusCode = StatusCodes.Status404NotFound;
                        }
                        else if(contextFeature.Error is BadRequestException)
                        {
                            context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        }
                        else
                        {
                            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        }
                        logger.LogError($"Something went wrong: {contextFeature.Error}");
                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = contextFeature.Error.Message,
                        }.ToString());
                    }
                });
            });
        }
        /*
context.Features: This is a collection of features associated with the current HTTP request. 
        Features are objects that provide additional functionalities or context-specific information.

Get<IExceptionHandlerFeature>(): This retrieves the feature of type IExceptionHandlerFeature from the features collection. 
        IExceptionHandlerFeature is an interface provided by ASP.NET Core that contains information about the 
        exception that was thrown during the request processing.

The IExceptionHandlerFeature interface provides access to the exception details that are captured by the UseExceptionHandler middleware. It has a property Error that contains the actual exception object.
         
The if statement ensures that you only attempt to log the error if the exception feature was successfully retrieved. 
It acts as a safety measure to avoid null reference exceptions and ensures that the logging and response logic only 
        executes when there is valid exception information.
If contextFeature were null, it would imply that there is no exception context available, possibly indicating 
        that the middleware was not configured correctly or an unexpected situation occurred. 
In such a case, logging or handling the error might be skipped, avoiding potential errors in the 
        error-handling logic itself.

         
         */

    }
}
