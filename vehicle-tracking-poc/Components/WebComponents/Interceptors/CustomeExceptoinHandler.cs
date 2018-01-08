using BuildingAspects.Behaviors;
using BuildingAspects.Services;
using DomainModels.Types;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            var messageHeader = new MessageHeader { CorrelateId = correlationId };

            var messageFooter = new MessageFooter
            {
                Sender = context.ActionDescriptor.DisplayName,
                Environemnt = _operationalUnit.Environment,
                Assembly = _operationalUnit.Assembly,
                FingerPrint = context.ActionDescriptor.Id,
                Route = context.RouteData.Values.ToDictionary(key => key.Key, value => value.Value?.ToString()),
                Hint = MessageHint.SystemError
            };

            context.ExceptionHandled = true;
            context.Result = new ContentResult()
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                ContentType = ContentTypes.ApplicationJson,
                Content = JsonConvert.SerializeObject(closureGenerateResponseMessage(), Utilities.JsonSerializerSettings)
            };

            return Task.CompletedTask;

            object closureGenerateResponseMessage()
            {
                return new
                {
                    header = messageHeader,
                    //TODO: system only should share user friendly messages, as well to not break system security.
                    body = context.Exception.Message,
                    footer = messageFooter
                };
            }

        }

    }
}
