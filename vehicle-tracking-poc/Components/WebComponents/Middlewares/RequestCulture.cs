using Microsoft.AspNetCore.Http;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace WebComponents.WebMiddlewares
{
    public class RequestCulture
    {
        private readonly RequestDelegate _next;

        public RequestCulture(RequestDelegate next)
        {
            _next = next;
        }
        public Task Invoke(HttpContext context)
        {
            var cultureQuery = context.Request.Query["culture"];
            if (!string.IsNullOrEmpty(cultureQuery))
            {
                var culture = new CultureInfo(cultureQuery) ?? CultureInfo.InvariantCulture;
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
            }
            return _next(context);
        }
    }
}
