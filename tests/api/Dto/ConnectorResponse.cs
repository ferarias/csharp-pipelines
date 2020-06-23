using System.Collections.Generic;

namespace Pipelines.ApiTests.Dto
{
    public class ConnectorResponse
    {
        public int ResponseId { get; set; }

        public IDictionary<int, bool> Availability { get; set; }
    }
}
