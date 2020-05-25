namespace pipelines
{
    public class IntToStringStep : IPipelineStep<int, string>
    {
        public string Process(int input)
        {
            return input.ToString();
        }
    }
}