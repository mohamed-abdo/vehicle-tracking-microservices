
using BuildingAspects.Behaviors;
using BuildingAspects.Services;
using DomainModels.Types;
using DomainModels.Types.Messages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
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
                Guid.TryParse(correlationHeader, out correlationId);

            //TODO: remove bypassing authorization
            if (false && string.IsNullOrEmpty(context.HttpContext.Request.Headers["authorization"]))
            {
                var messageHeader = new MessageHeader { CorrelateId = correlationId };

                var messageFooter = new MessageFooter
                {
                    Sender = context.ActionDescriptor.DisplayName,
                    Environemnt = _operationalUnit.Environment,
                    Assembly = _operationalUnit.Assembly,
                    FingerPrint = context.ActionDescriptor.Id,
                    Route = context.RouteData.Values.ToDictionary(key => key.Key, value => value.Value?.ToString()),
                    Hint = ResponseHint.UnAuthorized
                };

                //reject the request
                context.Result = new ContentResult()
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    ContentType = Identifiers.ApplicationJson,
                    Content = JsonConvert.SerializeObject(closureGenerateResponseMessage(), Utilities.DefaultJsonSerializerSettings)
                };
                return Task.CompletedTask;

                object closureGenerateResponseMessage()
                {
                    return new
                    {
                        header = messageHeader,
                        body = string.Empty,
                        footer = messageFooter
                    };
                }
            }
            //continue the pipeline
            return Task.CompletedTask;

        }
    }
}
