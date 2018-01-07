using DomainModels.Types;
using DomainModels.Vehicle;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace WebComponents.Interceptors
{
    public class CustomResponseResult : Attribute, IFilterFactory
    {
        public bool IsReusable => false;
        private readonly ILogger _logger;
        public CustomResponseResult(ILogger logger)
        {
            _logger = logger;
        }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return new AsyncCustomResponseResult(_logger);
        }
        private class AsyncCustomResponseResult : ResultFilterAttribute
        {
            private readonly ILogger _logger;
            public AsyncCustomResponseResult(ILogger logger)
            {
                _logger = logger;
            }
            public override Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
            {
                var rawContent = (context.Result as ContentResult).Content;
                context.Result = new ContentResult()
                {
                    StatusCode = StatusCodes.Status200OK,
                    ContentType = ContentTypes.ApplicationJson,
                    Content = JsonConvert.SerializeObject(new ResponseModel<PingModel>() { Body = new PingModel() { Message = "Hello world - microservices are here!!!" } })
                };
                _logger.LogInformation("Overriding response!");
                return base.OnResultExecutionAsync(context, next);
            }
        }
    }
}
