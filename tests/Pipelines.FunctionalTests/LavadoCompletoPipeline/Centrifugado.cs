using System;
using Serilog;

namespace Pipelines.FunctionalTests.LavadoCompletoPipeline
{
    public class Centrifugado : BasePipelineStep<double, int>
    {
        protected override int ProcessStep(double input)
        {
            var output = (int)Math.Round(input);
            Log.Debug("Centrifugando '{input}' [{itype}] ==> '{output}' [{otype}]",
                input, input.GetType().Name,
                output, output.GetType().Name);
            return output;
        }
    }
}
