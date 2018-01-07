using DomainModels.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace WebComponents.Interceptors
{
    public class CustomHeader : Attribute, IFilterFactory
    {
        public bool IsReusable => false;

        private readonly string _name, _value;
        public CustomHeader(string name, string value)
        {
            _name = name;
            _value = value;
        }
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return new AsyncCustomHeaderFilter(_name, _value);
        }

        private class AsyncCustomHeaderFilter : IAsyncActionFilter
        {
            private readonly string _name, _value;
            public AsyncCustomHeaderFilter(string name, string value)
            {
                _name = name;
                _value = value;
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
}
