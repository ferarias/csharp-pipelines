using Serilog;
using Pipelines.Extensions;

namespace Pipelines.FunctionalTests.LavadoCompletoPipeline
{
    public class LavadoCompleto : BasePipeline<string, int>
    {
        public LavadoCompleto()
        {
            Steps = input => input
            .AddStep(new Prelavado())
            .AddStep(new Lavado())
            .AddStep(new SubPipeline())
            .AddStep(new Aclarado(),
                    i => CustomLog("Pre-aclarado '{input}'...", i),
                    i => CustomLog("Post-aclarado '{input}'...", i))
            .AddStep(new Centrifugado());
        }

        private void CustomLog<T>(string messageTemplate, T value)
        {
            Log.Information(messageTemplate, value);
        }
    }
}
