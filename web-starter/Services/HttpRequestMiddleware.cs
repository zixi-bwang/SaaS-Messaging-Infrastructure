using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace web_starter.Services
{
    public class HttpRequestMiddleware
    {
        private readonly RequestDelegate _next;

        public HttpRequestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var httpRequest = httpContext.RequestServices.GetRequiredService<IHttpContextRequest>();
            var requestBody = await GetRequestBody(httpContext.Request);
            httpRequest.Body = requestBody;
            httpRequest.Path = httpContext.Request.Path;
            await _next(httpContext);
        }

        private async Task<string> GetRequestBody(HttpRequest request)
        {
            var requestBodyStream = new MemoryStream();
            var originalRequestBody = request.Body;

            await originalRequestBody.CopyToAsync(requestBodyStream);
            requestBodyStream.Seek(0, SeekOrigin.Begin);

            var requestBodyText = await (new StreamReader(requestBodyStream)).ReadToEndAsync();
            requestBodyStream.Seek(0, SeekOrigin.Begin);

            request.Body = requestBodyStream;

            return requestBodyText;

        }
    }
}