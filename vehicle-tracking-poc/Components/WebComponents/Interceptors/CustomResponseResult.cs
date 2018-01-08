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
    public class CustomResponseResult : IFilterFactory, IAsyncResultFilter
    {
        private readonly ILogger _logger;
        public CustomResponseResult(ILogger logger)
        {
            _logger = logger;
        }

        public bool IsReusable => false;
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return this;
        }

        public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var rawContent = (context.Result as ContentResult)?.Content;
            context.Result = new ContentResult()
            {
                StatusCode = StatusCodes.Status200OK,
                ContentType = ContentTypes.ApplicationJson,
                Content = JsonConvert.SerializeObject(new ResponseModel<PingModel>() { Body = new PingModel() { Message = $"Hello world - {rawContent} microservices are here!!!" } })
            };
            _logger.LogInformation("Overriding response!");
            return next.Invoke();
        }

    }
}
