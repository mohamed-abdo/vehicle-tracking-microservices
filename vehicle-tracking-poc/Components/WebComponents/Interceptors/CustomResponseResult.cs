using BuildingAspects.Behaviors;
using BuildingAspects.Services;
using DomainModels.Types;
using DomainModels.Types.Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
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
                correlationId = correlationHeader;
            var messageHeader = new MessageHeader(isSucceed: context.HttpContext.Response.StatusCode == 200) { CorrelationId = correlationId.ToString() };

            var messageFooter = new MessageFooter
            {
                Sender = context.ActionDescriptor.DisplayName,
                Environment = _operationalUnit.Environment,
                Assembly = _operationalUnit.Assembly,
                FingerPrint = context.ActionDescriptor.Id,
                Route = JsonConvert.SerializeObject(context.RouteData.Values.ToDictionary(key => key.Key, value => value.Value?.ToString()), Defaults.JsonSerializerSettings),
                Hint = Enum.GetName(typeof(ResponseHint), ResponseHint.OK)
                //TODO: infer the hint from HTTP status code
            };

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
                StatusCode = context.HttpContext.Response.StatusCode,
                ContentType = Identifiers.ApplicationJson,
                Content = JsonConvert.SerializeObject(getContentResult(context.Result), Defaults.JsonSerializerSettings)
            };
            _logger.LogInformation("Override response!");
            return next.Invoke();

            object getContentResult(IActionResult result)
            {
                switch (result)
                {
                    case ContentResult cr:
                        {
                            return cr?.Content;
                        }
                    case ObjectResult or:
                        {
                            return or?.Value;
                        }
                    default:
                        {
                            return result?.ToString();
                        }
                }
            };
        }

    }
}
