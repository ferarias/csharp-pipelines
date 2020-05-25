using System;

namespace pipelines
{

    public class DoubleStep : BasePipelineStep<int,int>
    {
        protected override int ProcessStep(int input)
        {
            return 2 * input;
        }
    }
}
