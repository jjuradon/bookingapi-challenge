using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Booking.Core.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Booking.API.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (NotFoundServiceException ex)
            {
                var httpException = new HttpException(ex.Message, ex, HttpStatusCode.NotFound);
                await HandleExceptionAsync(context, httpException, httpException.HttpStatusCode);
            }
            catch (ConflictServiceException ex)
            {
                var httpException = new HttpException(ex.Message, ex, HttpStatusCode.Conflict);
                await HandleExceptionAsync(context, httpException, httpException.HttpStatusCode);
            }
            catch (ServiceException ex)
            {
                var httpException = new HttpException(ex.Message, ex, HttpStatusCode.BadRequest);
                await HandleExceptionAsync(context, httpException, httpException.HttpStatusCode);
            }
            catch (Exception ex)
            {
                var httpException = new HttpException(ex.Message, ex, HttpStatusCode.InternalServerError);
                await HandleExceptionAsync(context, httpException);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode httpCode = HttpStatusCode.InternalServerError)
        {
            // EventLogger.Error(exception);

            var result = JsonSerializer.Serialize(new { error = exception.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)httpCode;
            return context.Response.WriteAsync(result);
        }

        public class HttpException : Exception
        {
            public HttpException(string message, HttpStatusCode httpStatusCode)
                : base(message)
            {
                HttpStatusCode = httpStatusCode;
            }

            public HttpException(string message, Exception exception, HttpStatusCode httpStatusCode)
              : base(message, exception)
            {
                HttpStatusCode = httpStatusCode;
            }

            public HttpStatusCode HttpStatusCode { get; private set; }
        }
    }
}