using System;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using infrastructure.Utilities;
using infrastructure.Utilities.Exceptions;
using infrastructure.Services;

namespace web_starter.Extensions.ErrorHandling
{
    public class ErrorHandlingFilter : IAsyncExceptionFilter
    {
        private readonly ILogger<ErrorHandlingFilter> _logger;
        private readonly IHttpContextRequest _httpContextRequest;
        private readonly TelemetryClient _telemetryClient;

        public ErrorHandlingFilter(ILogger<ErrorHandlingFilter> logger,
            IHttpContextRequest httpContextRequest,
            TelemetryClient telemetryClient)
        {
            _logger = logger;
            _httpContextRequest = httpContextRequest;
            _telemetryClient = telemetryClient;
        }

        public async Task OnExceptionAsync(ExceptionContext context)
        {
            AddTelemetryTrackException(context);
            var message = "Unable to handle request.";
            message = context.Exception.Message ?? message;

            int statusCode = GetStatusCode(context);

            if (context.Exception is ClientApiException clientApiException)
            {
                statusCode = (int)clientApiException.ExceptionHttpStatusCode;
            }

            _logger.LogError(context.Exception, message);

            ErrorResponse response;

            //skip sql error
            if (context.Exception is SqlException)
                response = new ErrorResponse("SqlException", "check log");
            else if (context.Exception.Message.Contains("LINQ expression"))
                response = new ErrorResponse("SqlException", "check log");
            else
                response = new ErrorResponse("SystemException", message);

            context.HttpContext.Response.StatusCode = statusCode;
            context.Result = new ObjectResult(response);
            context.Exception = null;

            await Task.CompletedTask;
        }

        private static int GetStatusCode(ExceptionContext context)
        {
            return context.Exception switch
            {
                DataNotFoundException => 404,
                _ => 500
            };
            
        }

        private void AddTelemetryTrackException(ExceptionContext context)
        {
            if (_httpContextRequest.Body.Contains("basic"))
            {
                _httpContextRequest.Body = "######";
            }

            var properties = new Dictionary<string, string>
            {
                { "RequestBody", _httpContextRequest.Body },
                { "ExceptionMessage", context.Exception.Message },
                { "MessageData", JsonConvert.SerializeObject(context.Exception.Data) }


            };

            _telemetryClient.TrackException(context.Exception, properties);
        }
    }
}
