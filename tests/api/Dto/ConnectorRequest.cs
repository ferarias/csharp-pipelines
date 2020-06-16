using System.Collections.Generic;

namespace Pipelines.ApiTests.Dto
{
    public class ConnectorRequest
    {
        public int Id { get; set; }

        public IEnumerable<int> Properties { get; set; }
    }
}
