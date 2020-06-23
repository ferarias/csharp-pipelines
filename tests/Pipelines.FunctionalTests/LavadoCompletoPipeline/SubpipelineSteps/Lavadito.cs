using Serilog;
using System.Text;

namespace Pipelines.FunctionalTests.LavadoCompletoPipeline.SubpipelineSteps
{
    public class Lavadito : IPipelineStep<byte[], string>
    {
        public string Process(byte[] input)
        {
            var output = Encoding.UTF8.GetString(input);
            Log.Debug("Lavandito '{input}' [{itype}] ==> '{output}' [{otype}]",
                input, input.GetType().Name,
                output, output.GetType().Name);
            return output;
        }
    }
}
