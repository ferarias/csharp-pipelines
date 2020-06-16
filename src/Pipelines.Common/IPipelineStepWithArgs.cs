namespace Pipelines
{
    public interface IPipelineStepWithArgs<TInput, TParam1, TOutput> : IPipelineStep<TInput, TOutput>
    {
        TOutput Process(TInput input, TParam1 arg1);
    }

    public interface IPipelineStepWithArgs<TInput, TParam1, TParam2, TOutput> : IPipelineStep<TInput, TOutput>
    {
        TOutput Process(TInput input, TParam1 arg1, TParam2 arg2);
    }

    public interface IPipelineStepWithArgs<TInput, TParam1, TParam2, TParam3, TOutput> : IPipelineStep<TInput, TOutput>
    {
        TOutput Process(TInput input, TParam1 arg1, TParam2 arg2, TParam3 arg3);
    }
}
