using Microsoft.Extensions.DependencyInjection;
using Pipelines.ApiTests.Dto;
using Pipelines.ApiTests.SearchSteps;
using Pipelines.ApiTests.SearchSteps.ConnectorSteps;
using Pipelines.Extensions;

namespace Pipelines.ApiTests
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBookingPipelines(this IServiceCollection services)
        {
            var searchPipeline = new Pipeline<HubRequest, HubResponse>(hubRq => hubRq
                .AddStep(new MappingStep())
                .AddStep(new Pipeline<ConnectorRequest, ConnectorResponse>(connRq => connRq
                    .AddStep(new GetRequestStep())
                    .AddStep(new GetResponseStep())
                    .AddStep(new AggregateStep()))
                )
                .AddStep(new DedupeStep()));

            return services.AddSingleton<IPipelineStep<HubRequest, HubResponse>>(searchPipeline);
            
        }
    }


}
