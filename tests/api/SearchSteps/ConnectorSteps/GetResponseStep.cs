using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Pipelines.ApiTests.Dto;

namespace Pipelines.ApiTests.SearchSteps
{
    public class GetResponseStep : IPipelineStepWithArgs<ProviderRequest, HubRequest, ProviderResponse>
    {
        private readonly ILogger<GetResponseStep> _logger;

        public GetResponseStep(ILogger<GetResponseStep> logger)
        {
            _logger = logger;
        }

        public ProviderResponse Process(ProviderRequest providerRq, HubRequest hubRq)
        {
            var providerResponse = new ProviderResponse
            {
                Id = providerRq.Id,
                Availability = providerRq.Properties.ToDictionary(x => x, y => (y.GetHashCode() % 2) == 0)
            };

            _logger.LogInformation("{input},{arg1} --> {output}", providerRq, hubRq, providerResponse);

            return providerResponse;
        }

        ProviderResponse IPipelineStep<ProviderRequest, ProviderResponse>.Process(ProviderRequest providerRq)
        {
            if (providerRq == null)
                throw new ArgumentNullException(nameof(providerRq));

            throw new ArgumentNullException("HubRequest");
        }
    }
}
