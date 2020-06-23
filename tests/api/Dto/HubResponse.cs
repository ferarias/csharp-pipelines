using System.Collections.Generic;

namespace Pipelines.ApiTests.Dto
{
    public class HubResponse
    {
        public int ResponseId { get; set; }

        public IDictionary<string, bool> Availability { get; set; }
    }
}
