using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
            var sp = services.BuildServiceProvider();
            var searchPipeline = new Pipeline<HubRequest, HubResponse>(hubRq => hubRq
                .AddStep(new MappingStep(sp.GetRequiredService<ILogger<MappingStep>>()))
                .AddStep(new Pipeline<ConnectorRequest, ConnectorResponse>(connRq => connRq
                    .AddStep(new GetRequestStep(sp.GetRequiredService<ILogger<GetRequestStep>>()), hubRq)
                    .AddStep(new GetResponseStep(sp.GetRequiredService<ILogger<GetResponseStep>>()), hubRq)
                    .AddStep(new AggregateStep(sp.GetRequiredService<ILogger<AggregateStep>>())))
                )
                .AddStep(new DedupeStep(sp.GetRequiredService<ILogger<DedupeStep>>())));

            return services.AddSingleton<IPipelineStep<HubRequest, HubResponse>>(searchPipeline);
        }
    }
}
