using System.Text.RegularExpressions;
using Pipelines.Common;
using Serilog;

namespace Pipelines.FunctionalTests.Steps
{
    public class Lavado : BasePipelineStep<string, string>
    {
        private static readonly Regex rgx = new Regex("[^0-9.-]");
        protected override string ProcessStep(string input)
        {
            var output = rgx.Replace(input, "");
            Log.Debug("Lavando '{input}' [{itype}] ==> '{output}' [{otype}]",
                input, input.GetType().Name,
                output, output.GetType().Name);
            return output;
        }
    }
}
