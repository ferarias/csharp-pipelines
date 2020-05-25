using System;
using System.Globalization;
using Pipelines.Common;

namespace Ferarias.MyPipeline
{
    public static class Program
    {
        private const int DefaultInputValue = 6;

        public static int Main(string[] args)
        {
            Console.WriteLine("Event Pipeline Test");

            var input = DefaultInputValue;
            if(args.Length > 0)
            {
                if (!int.TryParse(args[0], NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out input))
                    return -1;
            }

            Console.WriteLine($"Input Value: {input} [{input.GetType().Name}]");

            var pipeline = new TrivialPipeline();

            var output = pipeline.Process(input);

            Console.WriteLine($"Output Value: {output} [{output.GetType().Name}]");
            Console.WriteLine();

            return 0;
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
