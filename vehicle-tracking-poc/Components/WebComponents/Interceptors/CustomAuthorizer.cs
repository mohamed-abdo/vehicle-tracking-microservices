
using BuildingAspects.Behaviors;
using BuildingAspects.Services;
using DomainModels.Types;
using DomainModels.Types.Messages;
using Microsoft.AspNetCore.Http;
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
    public class CustomAuthorizer : IAsyncAuthorizationFilter, IFilterFactory
    {
        private readonly ILogger _logger;
        private readonly IOperationalUnit _operationalUnit;
        public CustomAuthorizer(ILogger logger, IOperationalUnit operationalUnit)
        {
            _logger = logger;
            _operationalUnit = operationalUnit;
        }

        public bool IsReusable => false;
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return this;
        }
        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            _logger.LogInformation("Authorize request header");
            var correlationHeader = context.HttpContext.Request.Headers[Identifiers.CorrelationId];
            //TODO:replace the following correlation id, since it's correlating all operation from this assembly instance.
            var correlationId = _operationalUnit.InstanceId;
            if (!string.IsNullOrEmpty(correlationHeader))
                correlationId = correlationHeader;

            //TODO: remove bypassing authorization
            if (false && string.IsNullOrEmpty(context.HttpContext.Request.Headers["authorization"]))
            {
                var messageHeader = new MessageHeader { CorrelationId = correlationId.ToString() };

                var messageFooter = new MessageFooter
                {
                    Sender = context.ActionDescriptor.DisplayName,
                    Environment = _operationalUnit.Environment,
                    Assembly = _operationalUnit.Assembly,
                    FingerPrint = context.ActionDescriptor.Id,
                    Route = JsonConvert.SerializeObject(context.RouteData.Values.ToDictionary(key => key.Key, value => value.Value?.ToString()), Defaults.JsonSerializerSettings),
                    Hint = Enum.GetName(typeof(ResponseHint), ResponseHint.UnAuthorized)
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
                    Content = JsonConvert.SerializeObject(Enum.GetName(typeof(ResponseHint), ResponseHint.UnAuthorized), Defaults.JsonSerializerSettings)
                };
                _logger.LogInformation("Override response!");
                return Task.CompletedTask;
            }
            return Task.CompletedTask;
        }
    }
}