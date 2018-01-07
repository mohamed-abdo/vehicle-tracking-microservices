using DomainModels.Types;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WebComponents.WebMiddlewares
{
    public class CustomResponse
    {
        private readonly RequestDelegate _next;
        public CustomResponse(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            context.Response.ContentType = new MediaTypeHeaderValue(ContentTypes.ApplicationJson)?.MediaType;
            using (var writer = new StreamWriter(context.Response.Body))
            {
                var json = new
                {
                    header = new ResponseHeader(),
                    body = "custom content"
                };
                var jsonStr = JsonConvert.SerializeObject(json, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Error,
                    PreserveReferencesHandling = PreserveReferencesHandling.Arrays,
                    TypeNameHandling = TypeNameHandling.Auto,
                    NullValueHandling = NullValueHandling.Include
                });

                await writer.WriteAsync(jsonStr);
            }
            await _next(context);
        }
    }
}
