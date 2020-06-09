namespace Pipelines
{
    /// <summary>
    /// Interface for steps. Get an input->process->return an output
    /// </summary>
    /// <typeparam name="TInput">Type of the input</typeparam>
    /// <typeparam name="TOutput">Type of the output</typeparam>
    public interface IPipelineStep<TInput, TOutput>
    {
        TOutput Process(TInput input);
    }
}
