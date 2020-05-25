using System;

namespace Pipelines.Common
{
    public abstract class Pipeline<TInput, TOutput>
    {
        public Func<TInput, TOutput> PipelineSteps { get; protected set; }

        public TOutput Process(TInput input)
        {
            return PipelineSteps(input);
        }
    }

}
