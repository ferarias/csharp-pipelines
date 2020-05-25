using System;

namespace Pipelines.Common
{
    /// <summary>
    /// Abstract implementation of a simple step.
    /// Delegates the process to a child class
    /// </summary>
    /// <typeparam name="TInput">Type of the input</typeparam>
    /// <typeparam name="TOutput">Type of the output</typeparam>
    /// <seealso cref="PipelineStep" />
    public abstract class BasePipelineStep<TInput, TOutput> : IPipelineStep<TInput, TOutput>
    {
        /// <summary>
        /// Action to invoke before processing input
        /// </summary>
        public event Action<TInput> OnInput;

        /// <summary>
        /// Action to invoke after processing output
        /// </summary>
        public event Action<TOutput> OnOutput;

        // note need for descendant types to implement this, not Process()
        protected abstract TOutput ProcessStep(TInput input);

        /// <summary>
        /// Execute the process step
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public TOutput Process(TInput input)
        {
            OnInput?.Invoke(input);

            var output = ProcessStep(input);

            OnOutput?.Invoke(output);

            return output;
        }
    }
}
