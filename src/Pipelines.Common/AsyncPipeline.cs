using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pipelines
{

    public class AsyncPipeline<TInput, TOutput> : BaseAsyncPipeline<TInput, TOutput>
    {
        public AsyncPipeline(Func<TInput, CancellationToken, Task<TOutput>> steps) : base(steps)
        {
        }
    }
}
