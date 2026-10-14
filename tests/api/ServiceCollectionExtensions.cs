using System.Linq;
using System.Threading.Tasks;
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
            // Resolve each step and assemble the pipeline from the real service provider, instead of
            // building a second, temporary one just to grab a few loggers.
            return services.AddSingleton<IPipelineStep<HubRequest, HubResponse>>(sp =>
            {
                var mapping = new MappingStep(sp.GetRequiredService<ILogger<MappingStep>>());
                var getRequest = new GetRequestStep(sp.GetRequiredService<ILogger<GetRequestStep>>());
                var getResponse = new GetResponseStep(sp.GetRequiredService<ILogger<GetResponseStep>>());
                var aggregate = new AggregateStep(sp.GetRequiredService<ILogger<AggregateStep>>());
                var dedupe = new DedupeStep(sp.GetRequiredService<ILogger<DedupeStep>>());

                // Build the pipeline using the steps, in the desired order
                return new Pipeline<HubRequest, HubResponse>(hubRq => hubRq
                    .AddStep(mapping)
                    // Sub-pipeline with three steps, two of them also use the input from parent
                    .AddStep(new Pipeline<ConnectorRequest, ConnectorResponse>(connRq => connRq
                        .AddStep(getRequest, hubRq)
                        .AddStep(getResponse, hubRq)
                        .AddStep(aggregate))
                    )
                    .AddStep(dedupe));
            });
        }

        public static IServiceCollection AddBookingPipelines2(this IServiceCollection services)
        {
            return services.AddSingleton<IAsyncPipelineStep<HubRequest, HubResponse>>(sp =>
            {
                var mapping = new MappingStep2();

                return new AsyncPipeline<HubRequest, HubResponse>(hubRq => hubRq
                    .AddStep(mapping));
            });
        }
        public class MappingStep2 : IAsyncPipelineStep<HubRequest, HubResponse>
        {
            public async Task<HubResponse> ProcessAsync(HubRequest hubRq)
            {
                await Task.Delay(1000).ConfigureAwait(false);
                return new HubResponse
                {
                    Id = hubRq.Id,
                    Availability = hubRq
                        .Properties
                        .ToDictionary(x => x, _ =>true)
                };
            }
        }
    }
}