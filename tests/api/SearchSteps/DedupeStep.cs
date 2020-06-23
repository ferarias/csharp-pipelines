using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Pipelines.ApiTests.Dto;

namespace Pipelines.ApiTests.SearchSteps
{
    public class DedupeStep : IPipelineStep<ConnectorResponse, HubResponse>
    {
        private readonly ILogger<DedupeStep> _logger;

        public DedupeStep(ILogger<DedupeStep> logger)
        {
            _logger = logger;
        }

        public HubResponse Process(ConnectorResponse input)
        {
            return new HubResponse
            {
                ResponseId = input.ResponseId,
                Availability = input.Availability.ToDictionary(x=> x.Key.ToString(), y=>y.Value)
            };
        }
    }
}
