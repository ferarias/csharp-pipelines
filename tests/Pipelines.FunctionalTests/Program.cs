using Serilog;

namespace Pipelines.FunctionalTests
{
    public static class Program
    {
        private const string DefaultInputValue = "-xxx***5.4asa";

        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.WithProcessId()
                .WriteTo.Console()
                .CreateLogger();

            Log.Information("Event Pipeline Test");

            var input = DefaultInputValue;
            if(args.Length > 0)
            {
                input = args[0];
            }

            Log.Information("Input Value: {value} [{type}]", input, input.GetType().Name);

            var pipeline = new LavadoCompleto();

            var output = pipeline.Process(input);

            Log.Information("Output Value: {value} [{type}]", output, output.GetType().Name);

            var output2 = pipeline.Process("pepe2");

            Log.Information("Output Value: {value} [{type}]", output2, output2.GetType().Name);

            return 0;
        }
    }
}
