using System;

namespace Pipelines.Extensions
{
    public static class PipelineStepEventExtensions
    {
        public static TOutput AddStep<TInput, TOutput>(
            this TInput input,
            IPipelineStep<TInput, TOutput> step)
        {
            return step.Process(input);
        }

        public static TOutput AddStep<TInput, TArg1, TOutput>(
            this TInput input,
            IPipelineStep<TInput, TArg1, TOutput> step, TArg1 arg1)
        {
            return step.Process(input, arg1);
        }

        public static TOutput AddStep<TInput, TOutput>(
            this TInput input,
            IPipelineStep<TInput, TOutput> step,
            Action<TInput> inputEvent = null,
            Action<TOutput> outputEvent = null)
        {
            var eventDecorator = new PipelineStep<TInput, TOutput>(step);
            eventDecorator.OnInput += inputEvent;
            eventDecorator.OnOutput += outputEvent;

            return eventDecorator.Process(input);
        }
    }
}
