using System;

namespace Pipelines.Common
{
    /// <summary>
    /// Implementation of a simple step
    /// Delegates the process to an internal IPipelineStep
    /// </summary>
    /// <typeparam name="TInput">Type of the input</typeparam>
    /// <typeparam name="TOutput">Type of the output</typeparam>
    /// <seealso cref="BasePipelineStep" />
    public class PipelineStep<TInput, TOutput> : IPipelineStep<TInput, TOutput>
    {
        /// <summary>
        /// Action to invoke before processing input
        /// </summary>
        public event Action<TInput> OnInput;

        /// <summary>
        /// Action to invoke after processing output
        /// </summary>
        public event Action<TOutput> OnOutput;

        /// <summary>
        /// The actual step to perform
        /// </summary>
        private readonly IPipelineStep<TInput, TOutput> _innerStep;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="innerStep">The actual step to perform</param>
        public PipelineStep(IPipelineStep<TInput,TOutput> innerStep)
        {
            _innerStep = innerStep;
        }

        /// <summary>
        /// The process that invokes the step and optionally invokes pre and post actions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public TOutput Process(TInput input)
        {
            OnInput?.Invoke(input);

            var output = _innerStep.Process(input);

            OnOutput?.Invoke(output);

            return output;
        }
    }
}
