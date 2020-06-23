using System;
using System.Threading.Tasks;

namespace Pipelines
{

    public class AsyncPipeline<TInput, TOutput> : BaseAsyncPipeline<TInput, TOutput>
    {
        public AsyncPipeline(Func<TInput, Task<TOutput>> steps)
        {
            Steps = steps;
        }
    }
}
