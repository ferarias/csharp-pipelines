using System;

namespace pipelines
{

    public interface IPipelineStep<INPUT, OUTPUT>
    {
        OUTPUT Process(INPUT input);
    }
}
