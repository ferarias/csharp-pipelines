using System;
using System.IO;

namespace pipelines
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Event Pipeline Test");

            var pipeline = new TrivialPipeline();

            var input = 6;
            Console.WriteLine(string.Format("Input Value: {0} [{1}]", input, input.GetType().Name));
            var output = pipeline.Process(input);

            Console.WriteLine(string.Format("Output Value: {0} [{1}]", output, output.GetType().Name));
            Console.WriteLine();
        }
    }

    public abstract class Pipeline<INPUT, OUTPUT>
    {
        public Func<INPUT, OUTPUT> PipelineSteps { get; protected set; }

        public OUTPUT Process(INPUT input)
        {
            return PipelineSteps(input);
        }
    }

    public class TrivialPipeline : Pipeline<int, int>
    {
        public TrivialPipeline()
        {
            var step1 =  new EventStep<int, int>(new DoubleStep());
            var step2 = new DoubleStep();
            PipelineSteps = input => input
            .Step(step1,
                    i => Console.WriteLine("Input event: " + i.ToString()),
                    i => Console.WriteLine("Input event: " + i.ToString()))
            .Step(step2);
        }
    }

}
