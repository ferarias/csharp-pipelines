using System;

namespace Pipelines.Common
{
    /// <summary>
    /// Abstract implementation of a pipeline
    /// </summary>
    /// <typeparam name="TInput">Type of the input</typeparam>
    /// <typeparam name="TOutput">Type of the output</typeparam>
    public abstract class Pipeline<TInput, TOutput>
    {
        /// <summary>
        /// A function that implements all the steps.
        /// </summary>
        /// <value></value>
        public Func<TInput, TOutput> Steps { get; protected set; }

        /// <summary>
        /// Process the steps. Invokes the pipeline steps.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public TOutput Process(TInput input)
        {
            return Steps(input);
        }
    }
}
