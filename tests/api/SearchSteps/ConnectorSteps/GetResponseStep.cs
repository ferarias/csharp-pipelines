using System;
using Pipelines.ApiTests.Dto;

namespace Pipelines.ApiTests.SearchSteps
{
    public class GetResponseStep : IPipelineStepWithArgs<ProviderRequest, HubRequest, ProviderResponse>
    {
        public ProviderResponse Process(ProviderRequest input, HubRequest hubRq)
        {
            return new ProviderResponse
            {
                Id = input.Id
            };
        }

        ProviderResponse IPipelineStep<ProviderRequest, ProviderResponse>.Process(ProviderRequest providerRq)
        {
            if (providerRq == null)
                throw new ArgumentNullException(nameof(providerRq));

            throw new ArgumentNullException("HubRequest");
        }
    }
}
