namespace Pipelines
{
    /// <summary>
    /// Interface for steps. Get an input->process->return an output
    /// </summary>
    /// <typeparam name="TInput">Type of the input</typeparam>
    /// <typeparam name="TOutput">Type of the output</typeparam>
    public interface IPipelineStep<TInput, TOutput>
    {
        /// <summary>
        /// Process the steps. Invokes the pipeline steps.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        TOutput Process(TInput input);
    }
}
