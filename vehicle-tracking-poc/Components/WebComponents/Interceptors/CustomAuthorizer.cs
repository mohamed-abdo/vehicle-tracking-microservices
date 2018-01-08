
using DomainModels.Types;
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
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WebComponents.Interceptors
{
    public class CustomAuthorizer : IAsyncAuthorizationFilter, IFilterFactory, IPolicyEvaluator
    {
        private readonly ILogger _logger;
        public CustomAuthorizer(ILogger logger)
        {
            _logger = logger;
        }

        public bool IsReusable => false;

        public Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
        {
            return new Task<AuthenticateResult>(() => AuthenticateResult.Success(new AuthenticationTicket(ClaimsPrincipal.Current, "Bearer")));
        }

        public Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy, AuthenticateResult authenticationResult, HttpContext context, object resource)
        {
            return new Task<PolicyAuthorizationResult>(() => PolicyAuthorizationResult.Success());
        }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return this;
        }
        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            _logger.LogInformation("Authorize request header");
            //TODO: remove bypassing authorization
            if (false && string.IsNullOrEmpty(context.HttpContext.Request.Headers["authorization"]))
            {
                //reject the request
                context.Result = new ContentResult()
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    ContentType = ContentTypes.ApplicationJson,
                    Content = JsonConvert.SerializeObject(new ResponseModel<string>() { Body = "No authorization header - microservices are here!!!" })
                };
                return Task.CompletedTask;
            }
            //continue the pipeline
            return Task.CompletedTask;
        }
    }
}
