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
        /// <returns>The output of the process</returns>
        TOutput Process(TInput input);
    }

    public interface IPipelineStep<TInput, TParam1, TOutput>
    {
        TOutput Process(TInput input, TParam1 arg1);
    }

    public interface IPipelineStep<TInput, TParam1, TParam2, TOutput>
    {
        TOutput Process(TInput input, TParam1 arg1, TParam2 arg2);
    }

    public interface IPipelineStep<TInput, TParam1, TParam2, TParam3, TOutput>
    {
        TOutput Process(TInput input, TParam1 arg1, TParam2 arg2, TParam3 arg3);
    }
}
