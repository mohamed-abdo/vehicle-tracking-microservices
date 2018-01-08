using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WebComponents.Interceptors
{
    public class CustomAuthonticator : IFilterFactory, IPolicyEvaluator

    {
        public bool IsReusable => false;
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return this;
        }

        public Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
        {
            return new Task<AuthenticateResult>(() => AuthenticateResult.Success(new AuthenticationTicket(ClaimsPrincipal.Current, "Bearer")));
        }

        public Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy, AuthenticateResult authenticationResult, HttpContext context, object resource)
        {
            return new Task<PolicyAuthorizationResult>(() => PolicyAuthorizationResult.Success());
        }
    }

}
