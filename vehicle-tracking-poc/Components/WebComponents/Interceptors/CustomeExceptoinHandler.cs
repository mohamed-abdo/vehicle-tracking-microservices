using DomainModels.Types;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebComponents.Interceptors
{
    public class CustomeExceptoinHandler : IAsyncExceptionFilter, IFilterFactory
    {
        public bool IsReusable => false;
        private readonly ILogger _logger;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IModelMetadataProvider _modelMetadataProvider;

        public CustomeExceptoinHandler(ILogger logger, IHostingEnvironment hostingEnvironment, IModelMetadataProvider modelMetadataProvider = null)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _modelMetadataProvider = modelMetadataProvider;
        }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return this;
        }

        public Task OnExceptionAsync(ExceptionContext context)
        {
            context.ExceptionHandled = true;
            context.Result = new ContentResult()
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                ContentType = ContentTypes.ApplicationJson,
                Content = JsonConvert.SerializeObject(new ResponseModel<string>()
                {
                    Body = $"App: {_hostingEnvironment.ApplicationName}, environment: {_hostingEnvironment.EnvironmentName}, exception: {context.Exception.Message}, caller: {context.ActionDescriptor.DisplayName}"
                })
            };
            _logger.LogInformation("Overriding response!");
            return Task.CompletedTask;
        }

    }
}
