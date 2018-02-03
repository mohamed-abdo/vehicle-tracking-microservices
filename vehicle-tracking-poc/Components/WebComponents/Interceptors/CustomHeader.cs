using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading.Tasks;

namespace WebComponents.Interceptors
{
	public class CustomHeader : Attribute, IFilterFactory, IAsyncActionFilter
    {
        private readonly string _name, _value;

        public CustomHeader(string name, string value = null)
        {
            _name = name;
            _value = value;
        }

        public bool IsReusable => false;
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return this;
        }

        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!string.IsNullOrEmpty(_name))
            {
                context.HttpContext.Response.Headers.Add(_name, new StringValues(_value));
            }
            return next.Invoke();
        }
    }
}
