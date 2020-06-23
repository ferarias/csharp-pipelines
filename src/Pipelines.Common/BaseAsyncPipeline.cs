using System;
using System.Threading.Tasks;

namespace Pipelines
{

    public abstract class BaseAsyncPipeline<TInput, TOutput> : IAsyncPipelineStep<TInput, TOutput>
    {
        public Func<TInput, Task<TOutput>> Steps { get; protected set; }

        public Task<TOutput> ProcessAsync(TInput input)
        {
            return Steps(input);
        }
    }
}
