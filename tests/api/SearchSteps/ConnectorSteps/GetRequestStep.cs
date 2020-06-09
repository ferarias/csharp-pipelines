using Pipelines.ApiTests.Dto;

namespace Pipelines.ApiTests.SearchSteps
{
    public class GetRequestStep : IPipelineStep<ConnectorRequest, ProviderRequest>
    {
        public ProviderRequest Process(ConnectorRequest input)
        {
            return new ProviderRequest
            {
                Id = input.Id
            };
        }
    }

}
