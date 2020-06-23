using System;

namespace Pipelines
{
    public class Pipeline<TInput, TOutput> : BasePipeline<TInput, TOutput>
    {
        public Pipeline(Func<TInput, TOutput> steps)
        {
            Steps = steps;
        }
    }
}
