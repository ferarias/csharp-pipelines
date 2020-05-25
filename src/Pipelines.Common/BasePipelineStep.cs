using System;

namespace Pipelines.Common
{
    /// <summary>
    /// Implementation of a simple step
    /// </summary>
    /// <typeparam name="TInput">Type of the input</typeparam>
    /// <typeparam name="TOutput">Type of the output</typeparam>
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

        public TOutput Process(TInput input)
        {
            OnInput?.Invoke(input);

            var output = ProcessStep(input);

            OnOutput?.Invoke(output);

            return output;
        }
    }
}
