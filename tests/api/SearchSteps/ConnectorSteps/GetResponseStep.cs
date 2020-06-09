using Pipelines.ApiTests.Dto;

namespace Pipelines.ApiTests.SearchSteps
{
    public class GetResponseStep : IPipelineStep<ProviderRequest, ProviderResponse>
    {
        public ProviderResponse Process(ProviderRequest input)
        {
            return new ProviderResponse
            {
                Id = input.Id
            };
        }
    }

}
