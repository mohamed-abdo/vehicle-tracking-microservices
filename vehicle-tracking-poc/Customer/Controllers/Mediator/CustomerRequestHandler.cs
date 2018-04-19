using BuildingAspects.Behaviors;
using DomainModels.Business.CustomerDomain;
using DomainModels.Business.VehicleDomain;
using DomainModels.Types.Messages;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Customer.Controllers.Mediator
{
    public class CustomerRequestHandler : IRequestHandler<CustomerRequest, IEnumerable<DomainModels.Business.CustomerDomain.Customer>>
    {
        private readonly ILogger<CustomerRequestHandler> _logger;
        public CustomerRequestHandler(ILogger<CustomerRequestHandler> logger)
        {
            _logger = logger;
        }
        public async Task<IEnumerable<DomainModels.Business.CustomerDomain.Customer>> Handle(CustomerRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Request => {nameof(CustomerRequest)}");
            //add correlation id
            request.Controller.HttpContext.Request.Headers.Add(DomainModels.Types.Identifiers.CorrelationId, new StringValues(request.CorrelationId.ToString()));

            var customerFilterModel = new CustomerFilterModel
            {
                Header = new MessageHeader
                {
                    CorrelationId = request.CorrelationId
                },
                Body = request.Model,
                Footer = new MessageFooter
                {
                    Sender = DomainModels.System.Identifiers.TrackingServiceName,
                    FingerPrint = request.Controller.ActionDescriptor.Id,
                    Environment = request.OperationalUnit.Environment,
                    Assembly = request.OperationalUnit.Assembly,
                    Route = JsonConvert.SerializeObject(new Dictionary<string, string> {
                                   { DomainModels.System.Identifiers.MessagePublisherRoute,  request.MiddlewareConfiguration.MessagePublisherRoute }
                             }, Defaults.JsonSerializerSettings),
                    Hint = Enum.GetName(typeof(ResponseHint), ResponseHint.OK)
                }
            };


            return await await new Function(_logger, DomainModels.System.Identifiers.RetryCount).Decorate(async () =>
            {
                var customer = await request.MessageRequest.Request(customerFilterModel);
                if (customer != null && customer.Any())
                {
                    //in case of data related data cached, or lookup joining, you may get and link data from previous data processed and cached.
                    //in case of the other request does not require data from th other (independent calls), you should call them in separate tasks and wait to consolidate the results.
                    var vehicleFilterModel = new VehicleFilterModel
                    {
                        Header = new MessageHeader
                        {
                            CorrelationId = request.CorrelationId
                        },
                        Body = new VehicleFilter { CustomerId = customer.FirstOrDefault().Id },
                        Footer = new MessageFooter
                        {
                            Sender = DomainModels.System.Identifiers.TrackingServiceName,
                            FingerPrint = request.Controller.ActionDescriptor.Id,
                            Environment = request.OperationalUnit.Environment,
                            Assembly = request.OperationalUnit.Assembly,
                            Route = JsonConvert.SerializeObject(new Dictionary<string, string> {
                                   { DomainModels.System.Identifiers.MessagePublisherRoute,  request.MiddlewareConfiguration.MessagePublisherRoute }
                             }, Defaults.JsonSerializerSettings),
                            Hint = Enum.GetName(typeof(ResponseHint), ResponseHint.OK)
                        }
                    };
                    var customerVehicles = await request.VehicleMessageRequest.Request(vehicleFilterModel);
                    if (customerVehicles != null && customerVehicles.Any())
                    {
                        customer.FirstOrDefault().Vehicles = new HashSet<Vehicle>(customerVehicles);
                    }
                }
                return customer;
            });
        }
    }
}
