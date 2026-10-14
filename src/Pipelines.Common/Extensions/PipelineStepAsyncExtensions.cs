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

        /// <summary>
        /// Await the result of a previous step and add a step to process it, transforming it into an output.
        /// Allows chaining several async steps together, e.g. input.AddStep(step1).AddStep(step2).
        /// </summary>
        /// <param name="inputTask">A task returning the input, typically the result of a previous AddStep call</param>
        /// <typeparam name="TInput">Type of the input</typeparam>
        /// <param name="step">Pipeline step</param>
        /// <typeparam name="TOutput">Type of the output</typeparam>
        /// <returns>Returns the output of processing the awaited input</returns>
        public static async Task<TOutput> AddStep<TInput, TOutput>(
            this Task<TInput> inputTask,
            IAsyncPipelineStep<TInput, TOutput> step)
        {
            var input = await inputTask.ConfigureAwait(false);
            return await step.ProcessAsync(input).ConfigureAwait(false);
        }

        /// <summary>
        /// Await the result of a previous step and add a step to process it, transforming it into an output.
        /// Allows chaining several async steps together, e.g. input.AddStep(step1).AddStep(step2, arg1).
        /// </summary>
        /// <param name="inputTask">A task returning the input, typically the result of a previous AddStep call</param>
        /// <typeparam name="TInput">Type of the input</typeparam>
        /// <param name="step">Pipeline step</param>
        /// <typeparam name="TOutput">Type of the output</typeparam>
        /// <param name="arg1">Additional argument passed to the process</param>
        /// <typeparam name="TArg1">Type of the first argument</typeparam>
        /// <returns>Returns the output of processing the awaited input</returns>
        public static async Task<TOutput> AddStep<TInput, TArg1, TOutput>(
            this Task<TInput> inputTask,
            IAsyncPipelineStep<TInput, TArg1, TOutput> step,
            TArg1 arg1)
        {
            var input = await inputTask.ConfigureAwait(false);
            return await step.ProcessAsync(input, arg1).ConfigureAwait(false);
        }

        /// <summary>
        /// Await the result of a previous step and add a step to process it, transforming it into an output.
        /// Allows chaining several async steps together.
        /// </summary>
        /// <param name="inputTask">A task returning the input, typically the result of a previous AddStep call</param>
        /// <typeparam name="TInput">Type of the input</typeparam>
        /// <param name="step">Pipeline step</param>
        /// <typeparam name="TOutput">Type of the output</typeparam>
        /// <param name="arg1">Additional argument passed to the process</param>
        /// <typeparam name="TArg1">Type of the first argument</typeparam>
        /// <param name="arg2">Additional argument passed to the process</param>
        /// <typeparam name="TArg2">Type of the second argument</typeparam>
        /// <returns>Returns the output of processing the awaited input</returns>
        public static async Task<TOutput> AddStep<TInput, TArg1, TArg2, TOutput>(
            this Task<TInput> inputTask,
            IAsyncPipelineStep<TInput, TArg1, TArg2, TOutput> step,
            TArg1 arg1, TArg2 arg2)
        {
            var input = await inputTask.ConfigureAwait(false);
            return await step.ProcessAsync(input, arg1, arg2).ConfigureAwait(false);
        }

        /// <summary>
        /// Await the result of a previous step and add a step to process it, transforming it into an output.
        /// Allows chaining several async steps together.
        /// </summary>
        /// <param name="inputTask">A task returning the input, typically the result of a previous AddStep call</param>
        /// <typeparam name="TInput">Type of the input</typeparam>
        /// <param name="step">Pipeline step</param>
        /// <typeparam name="TOutput">Type of the output</typeparam>
        /// <param name="arg1">Additional argument passed to the process</param>
        /// <typeparam name="TArg1">Type of the first argument</typeparam>
        /// <param name="arg2">Additional argument passed to the process</param>
        /// <typeparam name="TArg2">Type of the second argument</typeparam>
        /// <param name="arg3">Additional argument passed to the process</param>
        /// <typeparam name="TArg3">Type of the third argument</typeparam>
        /// <returns>Returns the output of processing the awaited input</returns>
        public static async Task<TOutput> AddStep<TInput, TArg1, TArg2, TArg3, TOutput>(
            this Task<TInput> inputTask,
            IAsyncPipelineStep<TInput, TArg1, TArg2, TArg3, TOutput> step,
            TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            var input = await inputTask.ConfigureAwait(false);
            return await step.ProcessAsync(input, arg1, arg2, arg3).ConfigureAwait(false);
        }
    }
}
