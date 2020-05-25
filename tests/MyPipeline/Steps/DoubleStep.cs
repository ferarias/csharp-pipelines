using Pipelines.Common;

namespace Ferarias.MyPipeline
{
    public class DoubleStep : BasePipelineStep<int,int>
    {
        protected override int ProcessStep(int input)
        {
            return 2 * input;
        }
    }
}
