namespace Pipelines.Common
{
    public interface IPipelineStep<TInput, TOutput>
    {
        TOutput Process(TInput input);
    }
}
