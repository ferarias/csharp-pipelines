using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Pipelines;

namespace Namespace
{
    public abstract class ConfigBasedPipeline<INPUT, OUTPUT> : BasePipeline<INPUT, OUTPUT>
    {
        private readonly ServiceProvider _sp;

        protected ConfigBasedPipeline(ServiceProvider sp, string pipelineJson)
        {
            if (pipelineJson == null)
            {
                throw new ArgumentNullException(nameof(pipelineJson));
            }
            _sp = sp;
            var pipelineSteps = ParsePipelineSteps(pipelineJson);
            ValidatePipelineSteps(pipelineSteps);

            Steps = input => ProcessPipelineSteps(pipelineSteps, input);
            
        }

        private OUTPUT ProcessPipelineSteps(IList<IPipelineStep> pipelineSteps, INPUT input)
        {
            object output = input;

            foreach (IPipelineStep step in pipelineSteps)
            {
                MethodInfo mi = step.GetType().GetMethod("Process", BindingFlags.Public | BindingFlags.Instance);
                output = mi.Invoke(step, new[] { output });
            }

            return (OUTPUT)output;
        }

        private IList<IPipelineStep> ParsePipelineSteps(string pipelineXml)
        {
            var pipeline = new List<IPipelineStep>();

            var steps = JsonSerializer.Deserialize<IEnumerable<string>>(pipelineXml);

            foreach (var xStep in steps)
            {
                string typeName = xStep;

                var type = Type.GetType(typeName);
                pipeline.Add(ResolveService(type));
            }

            return pipeline;
        }

        protected virtual IPipelineStep ResolveService(Type type) =>
            (IPipelineStep)_sp.GetRequiredService(type);

        private void ValidatePipelineSteps(IList<IPipelineStep> pipelineSteps)
        {
            int stepNumber = 0;

            Type pipelineBaseInterface = this.GetType().GetInterface("IPipelineStep`2");
            Type currentInputType = pipelineBaseInterface.GenericTypeArguments[0];
            Type outputType = pipelineBaseInterface.GenericTypeArguments[1];
            foreach (var step in pipelineSteps)
            {
                stepNumber++;

                Type stepBaseInterface = step.GetType().GetInterface("IPipelineStep`2");
                Type stepInType = stepBaseInterface.GenericTypeArguments[0];
                Type stepOutType = stepBaseInterface.GenericTypeArguments[1];

                if (currentInputType != stepInType)
                {
                    string msg = "Step #{0} {1} input type {2} does not match current type {3}.";
                    throw new InvalidOperationException(string.Format(msg, stepNumber, step.GetType().Name, stepInType.Name, currentInputType.Name));
                }
                currentInputType = stepOutType;
            }
            if (currentInputType != outputType)
            {
                string msg = "Final step #{0} {1} output type {2} does not equal output of pipeline {3}.";
                throw new InvalidOperationException(string.Format(msg, stepNumber, pipelineSteps.Last().GetType().Name, currentInputType.Name, outputType.Name));
            }
        }
    }
}