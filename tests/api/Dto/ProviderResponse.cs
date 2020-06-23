using System.Collections.Generic;

namespace Pipelines.ApiTests.Dto
{
    public class ProviderResponse
    {
        public int ResponseId { get; set; }

        public IDictionary<string, bool> Availability { get; set; }
    }
}
