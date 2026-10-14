using Pipelines.Extensions;

namespace Pipelines.FunctionalTests.LavadoCompletoPipeline
{
    public class SubPipeline : BasePipeline<string, string>
    {
        public SubPipeline() : base(input => input
            .AddStep(new SubpipelineSteps.Prelavadito())
            .AddStep(new SubpipelineSteps.Lavadito()))
        {
        }
    }
}
