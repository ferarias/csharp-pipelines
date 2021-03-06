using System;

namespace Pipelines
{
    /// <summary>
    /// Abstract implementation of a pipeline
    /// </summary>
    /// <typeparam name="TInput">Type of the input</typeparam>
    /// <typeparam name="TOutput">Type of the output</typeparam>
    public abstract class BasePipeline<TInput, TOutput> : IPipelineStep<TInput, TOutput>
    {
        /// <summary>
        /// A function that contains all the steps.
        /// </summary>
        /// <value>A function accepting TInput and returning TOutput</value>
        public Func<TInput, TOutput> Steps { get; protected set; }

        /// <summary>
        /// Process the steps. Invokes the pipeline steps.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>The output of processing input</returns>
        public TOutput Process(TInput input)
        {
            return Steps(input);
        }
    }
}
