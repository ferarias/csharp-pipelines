using Pipelines.Common;
using Serilog;

namespace Pipelines.FunctionalTests.Steps
{
    public class Aclarado : BasePipelineStep<string, double>
    {
        protected override double ProcessStep(string input)
        {
            var output = double.Parse(input);
            Log.Debug("Aclarando '{input}' [{itype}] ==> '{output}' [{otype}]",
                input, input.GetType().Name,
                output, output.GetType().Name);
            return output;
        }
    }
}
