using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PocApiNetCore.Extensoes
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuracoes;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> Ilogger, IConfiguration configuracoes)
        {
            _next = next;
            _logger = Ilogger;
            _configuracoes = configuracoes;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var request = string.Empty;

            try
            {
                ////First, get the incoming request
                request = await FormatRequest(httpContext.Request);

                ////Copy a pointer to the original response body stream
                var originalBodyStream = httpContext.Response.Body;

                //Continue down the Middleware pipeline, eventually returning to this class
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandlerExceptionAsync(httpContext, ex, request);
            }
        }

        private async Task HandlerExceptionAsync(HttpContext context, Exception exception, string request)
        {
            var messageError = $"Data: {DateTime.Now} \n " +
               $"Request: {context.Request.Path.ToString() } \n " +
               $"Corpo da request: {request} \n " +
               $"Source: {exception.Source} \n " +
               $"StackTrace: {exception.StackTrace} \n " +
               $"Message: {exception.Message} \n ";

            _logger.LogCritical(messageError);

            context.Response.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.HttpContext.Response.ContentType = "plain/text";
            await context.Response.HttpContext.Response.WriteAsync("Ops! Algo deu errado, tente novamente mais tarde.");
        }

        private async Task<string> FormatRequest(HttpRequest context)
        {
            var body = context.Body;

            //This line allows us to set the reader for the request back at the beginning of its stream.
            context.EnableRewind();

            //We now need to read the request stream.  First, we create a new byte[] with the same length as the request stream...
            var buffer = new byte[Convert.ToInt32(context.ContentLength)];

            //...Then we copy the entire request stream into the new buffer.
            await context.Body.ReadAsync(buffer, 0, buffer.Length);

            //We convert the byte[] into a string using UTF8 encoding...
            var bodyAsText = Encoding.UTF8.GetString(buffer);

            context.Body.Seek(0, SeekOrigin.Begin);

            //..and finally, assign the read body back to the request body, which is allowed because of EnableRewind()
            context.Body = body;

            return $"{context.Scheme} {context.Host}{context.Path} {context.QueryString} {bodyAsText}";
        }
    }

    public static class ExcecoesMiddleware
    {
        public static IApplicationBuilder UseExcecoesMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();

            return app;
        }
    }
}
