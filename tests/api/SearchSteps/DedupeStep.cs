using Pipelines.ApiTests.Dto;

namespace Pipelines.ApiTests.SearchSteps
{
    public class DedupeStep : IPipelineStep<ConnectorResponse, HubResponse>
    {
        public HubResponse Process(ConnectorResponse input)
        {
            return new HubResponse
            {
                Id = input.Id
            };
        }
    }


}
