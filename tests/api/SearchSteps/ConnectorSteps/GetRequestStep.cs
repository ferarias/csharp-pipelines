using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Pipelines.ApiTests.Dto;

namespace Pipelines.ApiTests.SearchSteps
{
    public class GetRequestStep : IPipelineStepWithArgs<ConnectorRequest, HubRequest, ProviderRequest>
    {
        private readonly ILogger<GetRequestStep> _logger;

        public GetRequestStep(ILogger<GetRequestStep> logger)
        {
            _logger = logger;
        }

        public ProviderRequest Process(ConnectorRequest connectorRq, HubRequest hubRq)
        {
            return new ProviderRequest
            {
                Id = connectorRq.Id,
                Properties = connectorRq.Properties.Select(x => $"{hubRq.HubExtraInfo}-{x}")
            };
        }

        public ProviderRequest Process(ConnectorRequest connectorRequest)
        {
            throw new ArgumentNullException("HubRequest");
        }
    }
}
