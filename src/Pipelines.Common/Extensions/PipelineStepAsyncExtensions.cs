using System.Threading.Tasks;

namespace Pipelines.Extensions
{
    public static class PipelineStepAsyncExtensions
    {
        /// <summary>
        /// Add a step to process the input and transform it into an output
        /// </summary>
        /// <param name="input">Input</param>
        /// <typeparam name="TInput">Type of the input</typeparam>
        /// <param name="step">Pipeline step</param>
        /// <typeparam name="TOutput">Type of the output</typeparam>
        /// <returns>Returns the output of processing the input</returns>
        public static Task<TOutput> AddStep<TInput, TOutput>(
            this TInput input,
            IAsyncPipelineStep<TInput, TOutput> step)
        {
            return step.ProcessAsync(input);
        }

        /// <summary>
        /// Add a step to process the input and transform it into an output
        /// </summary>
        /// <param name="input">Input</param>
        /// <typeparam name="TInput">Type of the input</typeparam>
        /// <param name="step">Pipeline step</param>
        /// <typeparam name="TOutput">Type of the output</typeparam>
        /// <param name="arg1">Additional argument passed to the process</param>
        /// <typeparam name="TArg1">Type of the first argument</typeparam>
        /// <returns>Returns the output of processing the input</returns>
        public static Task<TOutput> AddStep<TInput, TArg1, TOutput>(
            this TInput input,
            IAsyncPipelineStep<TInput, TArg1, TOutput> step,
            TArg1 arg1)
        {
            return step.ProcessAsync(input, arg1);
        }

        /// <summary>
        /// Add a step to process the input and transform it into an output
        /// </summary>
        /// <param name="input">Input</param>
        /// <typeparam name="TInput">Type of the input</typeparam>
        /// <param name="step">Pipeline step</param>
        /// <typeparam name="TOutput">Type of the output</typeparam>
        /// <param name="arg1">Additional argument passed to the process</param>
        /// <typeparam name="TArg1">Type of the first argument</typeparam>
        /// <param name="arg2">Additional argument passed to the process</param>
        /// <typeparam name="TArg1">Type of the second argument</typeparam>
        /// <returns>Returns the output of processing the input</returns>
        public static Task<TOutput> AddStep<TInput, TArg1, TArg2, TOutput>(
            this TInput input,
            IAsyncPipelineStep<TInput, TArg1, TArg2, TOutput> step, 
            TArg1 arg1, TArg2 arg2)
        {
            return step.ProcessAsync(input, arg1, arg2);
        }
        /// <summary>
        /// Add a step to process the input and transform it into an output
        /// </summary>
        /// <param name="input">Input</param>
        /// <typeparam name="TInput">Type of the input</typeparam>
        /// <param name="step">Pipeline step</param>
        /// <typeparam name="TOutput">Type of the output</typeparam>
        /// <param name="arg1">Additional argument passed to the process</param>
        /// <typeparam name="TArg1">Type of the first argument</typeparam>
        /// <param name="arg2">Additional argument passed to the process</param>
        /// <typeparam name="TArg1">Type of the second argument</typeparam>
        /// <param name="arg3">Additional argument passed to the process</param>
        /// <typeparam name="TArg3">Type of the third argument</typeparam>
        /// <returns>Returns the output of processing the input</returns>
        public static Task<TOutput> AddStep<TInput, TArg1, TArg2, TArg3, TOutput>(
            this TInput input,
            IAsyncPipelineStep<TInput, TArg1, TArg2, TArg3, TOutput> step,
            TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            return step.ProcessAsync(input, arg1, arg2, arg3);
        }
    }
}
