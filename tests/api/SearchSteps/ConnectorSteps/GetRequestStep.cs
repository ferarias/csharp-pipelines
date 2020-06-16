using System;
using Pipelines.ApiTests.Dto;

namespace Pipelines.ApiTests.SearchSteps
{
    public class GetRequestStep : IPipelineStepWithArgs<ConnectorRequest, HubRequest, ProviderRequest>
    {
        public ProviderRequest Process(ConnectorRequest connectorRq, HubRequest hubRq)
        {
            return new ProviderRequest
            {
                Id = connectorRq.Id
            };
        }

        public ProviderRequest Process(ConnectorRequest connectorRequest)
        {
            throw new ArgumentNullException("HubRequest");
        }
    }
}
