using System.Linq;
using Microsoft.Extensions.Logging;
using Pipelines.ApiTests.Dto;

namespace Pipelines.ApiTests.SearchSteps.ConnectorSteps
{
    public class AggregateStep : IPipelineStep<ProviderResponse, ConnectorResponse>
    {
        private readonly ILogger<AggregateStep> _logger;

        public AggregateStep(ILogger<AggregateStep> logger)
        {
            _logger = logger;
        }

        public ConnectorResponse Process(ProviderResponse input)
        {
            return new ConnectorResponse
            {
                ResponseId = input.ResponseId,
                Availability = input.Availability.ToDictionary(x => x.Key.GetHashCode(), y => y.Value)
            };
        }
    }
}