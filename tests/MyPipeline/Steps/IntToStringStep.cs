using Pipelines.Common;

namespace Ferarias.MyPipeline
{
    public class IntToStringStep : IPipelineStep<int, string>
    {
        public string Process(int input)
        {
            return input.ToString();
        }
    }
}