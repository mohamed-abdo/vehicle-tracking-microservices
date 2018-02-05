using BuildingAspects.Behaviors;
using BuildingAspects.Services;
using DomainModels.Types;
using DomainModels.Types.Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebComponents.Interceptors
{
    public class CustomResponseResult : IFilterFactory, IAsyncResultFilter
    {
        private readonly ILogger _logger;
        private readonly IOperationalUnit _operationalUnit;
        public CustomResponseResult(ILogger logger, IOperationalUnit operationalUnit)
        {
            _logger = logger;
            _operationalUnit = operationalUnit;
        }

        public bool IsReusable => false;
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return this;
        }

        public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var correlationHeader = context.HttpContext.Request.Headers[Identifiers.CorrelationId];
            //TODO:replace the following correlation id, since it's correlating all operation from this assembly instance.
            var correlationId = _operationalUnit.InstanceId;
            if (!string.IsNullOrEmpty(correlationHeader))
                Guid.TryParse(correlationHeader, out correlationId);
            var messageHeader = new MessageHeader(isSucceed: context.HttpContext.Response.StatusCode == 200) { CorrelationId = correlationId };

            var messageFooter = new MessageFooter
            {
                Sender = context.ActionDescriptor.DisplayName,
                Environment = _operationalUnit.Environment,
                Assembly = _operationalUnit.Assembly,
                FingerPrint = context.ActionDescriptor.Id,
                Route = context.RouteData.Values.ToDictionary(key => key.Key, value => value.Value?.ToString()),
                //TODO: infer the hint from HTTP status code
                Hint = ResponseHint.OK
            };


            var rawContent = (context.Result as ContentResult)?.Content;
            context.Result = new ContentResult()
            {
                StatusCode = context.HttpContext.Response.StatusCode,
                ContentType = Identifiers.ApplicationJson,
                Content = JsonConvert.SerializeObject(closureGenerateResponseMessage(), Utilities.DefaultJsonSerializerSettings)
            };
            _logger.LogInformation("Overriding response!");
            return next.Invoke();

            object closureGenerateResponseMessage()
            {
                return new
                {
                    header = messageHeader,
                    body = rawContent,
                    footer = messageFooter
                };
            }
        }

    }
}
