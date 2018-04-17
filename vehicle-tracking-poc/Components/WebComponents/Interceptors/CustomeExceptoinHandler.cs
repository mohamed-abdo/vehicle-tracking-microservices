using BuildingAspects.Behaviors;
using BuildingAspects.Services;
using DomainModels.Types;
using DomainModels.Types.Exceptions;
using DomainModels.Types.Messages;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebComponents.Interceptors
{
    public class CustomeExceptoinHandler : IFilterFactory, IAsyncExceptionFilter
    {

        private readonly ILogger _logger;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IModelMetadataProvider _modelMetadataProvider;
        private readonly IOperationalUnit _operationalUnit;
        public CustomeExceptoinHandler(ILogger logger, IOperationalUnit operationalUnit, IHostingEnvironment hostingEnvironment, IModelMetadataProvider modelMetadataProvider = null)
        {
            _logger = logger;
            _operationalUnit = operationalUnit;
            _hostingEnvironment = hostingEnvironment;
            _modelMetadataProvider = modelMetadataProvider;
        }

        public bool IsReusable => false;
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return this;
        }

        public Task OnExceptionAsync(ExceptionContext context)
        {
            _logger.LogCritical(context.Exception, context.Exception.Message);

            var correlationHeader = context.HttpContext.Request.Headers[Identifiers.CorrelationId];
            //TODO:replace the following correlation id, since it's correlating all operation from this assembly instance.
            var correlationId = _operationalUnit.InstanceId;
            if (!string.IsNullOrEmpty(correlationHeader) && Guid.TryParse(correlationHeader, out Guid paresedCorId))
                correlationId = paresedCorId;

            var exception = context.Exception;
            (int code, string message, ResponseHint responseHint) = (exception is CustomException ex) ? ex.CustomMessage : (exception.HResult, exception.Message, ResponseHint.SystemError);

            var messageHeader = new MessageHeader( isSucceed: false) { CorrelationId = correlationId };

            var messageFooter = new MessageFooter
            {
                Sender = context.ActionDescriptor.DisplayName,
                Environment = _operationalUnit.Environment,
                Assembly = _operationalUnit.Assembly,
                FingerPrint = context.ActionDescriptor.Id,
                Route = JsonConvert.SerializeObject(context.RouteData.Values.ToDictionary(key => key.Key, value => value.Value?.ToString()), Defaults.JsonSerializerSettings),
                Hint = Enum.GetName(typeof(ResponseHint), responseHint)
            };
            context.ExceptionHandled = true;
            messageHeader.GetType().GetProperties()
                 .ToList().ForEach(prop =>
                 {
                     context.HttpContext.Response.Headers.Add(prop.Name, new StringValues(prop.GetValue(messageHeader)?.ToString()));
                 });
            messageFooter.GetType().GetProperties()
                   .ToList().ForEach(prop =>
                   {
                       context.HttpContext.Response.Headers.Add(prop.Name, new StringValues(prop.GetValue(messageFooter)?.ToString()));
                   });

            context.Result = new ContentResult()
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                ContentType = Identifiers.ApplicationJson,
                Content = JsonConvert.SerializeObject(closureGenerateErrorMessage(), Defaults.JsonSerializerSettings)
            };

            _logger.LogInformation("Override response!");
            return Task.CompletedTask;
            object closureGenerateErrorMessage()
            {
                return new
                {
                    //TODO: system message only should share user friendly messages for system messages, as well to not break system security.
                    code,
                    message,
                    systemMessage = exception.Message
                };
            }
        }

    }
}
