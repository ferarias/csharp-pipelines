using Microsoft.Extensions.Logging;
using Pipelines.ApiTests.Dto;
using System.Linq;

namespace Pipelines.ApiTests.SearchSteps
{
    public class MappingStep : IPipelineStep<HubRequest, ConnectorRequest>
    {
        private readonly ILogger<MappingStep> _logger;

        public MappingStep(ILogger<MappingStep> logger)
        {
            _logger = logger;
        }

        public ConnectorRequest Process(HubRequest hubRq)
        {
            return new ConnectorRequest
            {
                Id = hubRq.Id,
                Properties = hubRq
                    .Properties
                    .Select(i => int.Parse(i) + 1000)
            };
        }
    }
}
