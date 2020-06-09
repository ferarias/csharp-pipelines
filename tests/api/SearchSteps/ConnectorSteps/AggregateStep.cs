using Pipelines.ApiTests.Dto;

namespace Pipelines.ApiTests.SearchSteps.ConnectorSteps
{
    public class AggregateStep : IPipelineStep<ProviderResponse, ConnectorResponse>
    {
        public ConnectorResponse Process(ProviderResponse input)
        {
            return new ConnectorResponse
            {
                Id = input.Id
            };
        }
    }
}