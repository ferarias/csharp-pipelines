using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pipelines
{

    public abstract class BaseAsyncPipeline<TInput, TOutput> : IAsyncPipelineStep<TInput, TOutput>
    {
        public Func<TInput, CancellationToken, Task<TOutput>> Steps { get; }

        protected BaseAsyncPipeline(Func<TInput, CancellationToken, Task<TOutput>> steps)
        {
            Steps = steps ?? throw new ArgumentNullException(nameof(steps));
        }

        public Task<TOutput> ProcessAsync(TInput input, CancellationToken cancellationToken = default)
        {
            return Steps(input, cancellationToken);
        }
    }
}
