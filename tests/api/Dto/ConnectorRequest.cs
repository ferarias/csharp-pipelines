using System.Collections.Generic;

namespace Pipelines.ApiTests.Dto
{
    public class ConnectorRequest
    {
        public int RequestId { get; set; }

        public IEnumerable<int> Properties { get; set; }
    }
}
