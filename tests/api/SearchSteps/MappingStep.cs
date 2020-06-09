using System.Text;
using Pipelines.ApiTests.Dto;

namespace Pipelines.ApiTests.SearchSteps
{
    public class MappingStep : IPipelineStep<HubRequest, ConnectorRequest>
    {
        public ConnectorRequest Process(HubRequest input)
        {
            return new ConnectorRequest
            {
                Id = input.Id
            };
        }
    }
}
