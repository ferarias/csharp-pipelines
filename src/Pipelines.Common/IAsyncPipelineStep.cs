using System.Threading;
using System.Threading.Tasks;

namespace Pipelines
{
    public interface IAsyncPipelineStep<TInput, TOutput>
    {
        Task<TOutput> ProcessAsync(TInput input, CancellationToken cancellationToken);
    }

    public interface IAsyncPipelineStep<TInput, TParam1, TOutput>
    {
        Task<TOutput> ProcessAsync(TInput input, TParam1 arg1, CancellationToken cancellationToken);
    }

    public interface IAsyncPipelineStep<TInput, TParam1, TParam2, TOutput>
    {
        Task<TOutput> ProcessAsync(TInput input, TParam1 arg1, TParam2 arg2, CancellationToken cancellationToken);
    }

    public interface IAsyncPipelineStep<TInput, TParam1, TParam2, TParam3, TOutput>
    {
        Task<TOutput> ProcessAsync(TInput input, TParam1 arg1, TParam2 arg2, TParam3 arg3, CancellationToken cancellationToken);
    }
}
