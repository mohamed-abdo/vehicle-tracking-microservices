using BuildingAspects.Behaviors;
using DomainModels.Types;
using DomainModels.Types.Messages;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
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
            var messageHeader = new MessageHeader();

            var messageFooter = new MessageFooter
            {
                Sender = context.Request.Path,
                //TODO: get request finger print
                FingerPrint = context.Request.Path,
                Route = context.Request.Query.ToDictionary(key => key.Key, vallue => vallue.Value.FirstOrDefault()),
                Hint = ResponseHint.Custom
            };

            context.Response.ContentType = new MediaTypeHeaderValue(Identifiers.ApplicationJson)?.MediaType;
            using (var writer = new StreamWriter(context.Response.Body))
            {
                var content = JsonConvert.SerializeObject(closureGenerateResponseMessage(), Utilities.DefaultJsonSerializerSettings);
                await writer.WriteAsync(content);
            }
            await _next(context);
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
    }
}
