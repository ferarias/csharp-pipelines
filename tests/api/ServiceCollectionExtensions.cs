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
            var sp = services.BuildServiceProvider();

            // Create each step of the pipeline
            var mapping = new MappingStep(sp.GetRequiredService<ILogger<MappingStep>>());
            var getRequest = new GetRequestStep(sp.GetRequiredService<ILogger<GetRequestStep>>());
            var getResponse = new GetResponseStep(sp.GetRequiredService<ILogger<GetResponseStep>>());
            var aggregate = new AggregateStep(sp.GetRequiredService<ILogger<AggregateStep>>());
            var dedupe = new DedupeStep(sp.GetRequiredService<ILogger<DedupeStep>>());

            // Build the pipeline using the steps, in the desired order
            var searchPipeline = new Pipeline<HubRequest, HubResponse>(hubRq => hubRq
                .AddStep(mapping)
                // Sub-pipeline with three steps, two of them also use the input from parent
                .AddStep(new Pipeline<ConnectorRequest, ConnectorResponse>(connRq => connRq
                    .AddStep(getRequest, hubRq)
                    .AddStep(getResponse, hubRq)
                    .AddStep(aggregate))
                )
                .AddStep(dedupe));

            // Inject this pipeline to the service provider (it is used in the controller)
            return services.AddSingleton<IPipelineStep<HubRequest, HubResponse>>(searchPipeline);
        }

        public static IServiceCollection AddBookingPipelines2(this IServiceCollection services)
        {
            var mapping = new MappingStep2();

            var searchPipelineAsync = new AsyncPipeline<HubRequest, HubResponse>(hubRq => hubRq
               .AddStep(mapping));

            return services;
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