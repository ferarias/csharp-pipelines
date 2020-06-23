using Serilog;

namespace Pipelines.FunctionalTests.LavadoCompletoPipeline
{
    public class Prelavado : IPipelineStep<string, string>
    {
        public string Process(string input)
        {
            var output = input.ToUpper();
            Log.Debug("Pre-lavando '{input}' [{itype}] ==> '{output}' [{otype}]",
                input, input.GetType().Name,
                output, output.GetType().Name);
            return output;
        }
    }
}