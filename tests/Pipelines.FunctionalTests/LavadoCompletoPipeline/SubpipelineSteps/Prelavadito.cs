using Serilog;
using System.Text;

namespace Pipelines.FunctionalTests.LavadoCompletoPipeline.SubpipelineSteps
{
    public class Prelavadito : IPipelineStep<string, byte[]>
    {
        public byte[] Process(string input)
        {
            var output = input.ToUpper();
            Log.Debug("Pre-lavandito '{input}' [{itype}] ==> '{output}' [{otype}]",
                input, input.GetType().Name,
                output, output.GetType().Name);
            return Encoding.UTF8.GetBytes(output);
        }
    }
}
