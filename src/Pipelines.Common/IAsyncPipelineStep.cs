using System.Threading.Tasks;

namespace Pipelines
{
    public interface IAsyncPipelineStep<TInput, TOutput>
    {
        Task<TOutput> ProcessAsync(TInput input);
    }

    public interface IAsyncPipelineStep<TInput, TParam1, TOutput> : IPipelineStep<TInput, TOutput>
    {
        Task<TOutput> ProcessAsync(TInput input, TParam1 arg1);
    }

    public interface IAsyncPipelineStep<TInput, TParam1, TParam2, TOutput> : IPipelineStep<TInput, TOutput>
    {
        Task<TOutput> ProcessAsync(TInput input, TParam1 arg1, TParam2 arg2);
    }

    public interface IAsyncPipelineStep<TInput, TParam1, TParam2, TParam3, TOutput> : IPipelineStep<TInput, TOutput>
    {
        Task<TOutput> ProcessAsync(TInput input, TParam1 arg1, TParam2 arg2, TParam3 arg3);
    }
}
