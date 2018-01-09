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
            if (!string.IsNullOrEmpty(correlationHeader))
                Guid.TryParse(correlationHeader, out correlationId);

            var exception = context.Exception;
            (int code, string message, ResponseHint responseHint) = (exception is CustomException ex) ? ex.CustomMessage : (exception.HResult, exception.Message, ResponseHint.SystemError);

            var messageHeader = new MessageHeader(isSucceed: false) { CorrelateId = correlationId };

            var messageFooter = new MessageFooter
            {
                Sender = context.ActionDescriptor.DisplayName,
                Environemnt = _operationalUnit.Environment,
                Assembly = _operationalUnit.Assembly,
                FingerPrint = context.ActionDescriptor.Id,
                Route = context.RouteData.Values.ToDictionary(key => key.Key, value => value.Value?.ToString()),
                Hint = responseHint
            };
            context.ExceptionHandled = true;
            context.Result = new ContentResult()
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                ContentType = Identifiers.ApplicationJson,
                Content = JsonConvert.SerializeObject(closureGenerateErrorMessage(), Utilities.DefaultJsonSerializerSettings)
            };

            return Task.CompletedTask;

            object closureGenerateErrorMessage()
            {
                return new
                {
                    header = messageHeader,
                    //TODO: system message only should share user friendly messages for system messages, as well to not break system security.
                    body = new
                    {
                        code,
                        message,
                        systemMessage = exception.Message
                    },
                    footer = messageFooter
                };
            }

        }

    }
}
