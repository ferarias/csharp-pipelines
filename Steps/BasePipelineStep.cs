using System;

namespace pipelines
{
    public abstract class BasePipelineStep<TInput, TOutput> : IPipelineStep<TInput, TOutput>
    {
        public event Action<TInput> OnInput;
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
