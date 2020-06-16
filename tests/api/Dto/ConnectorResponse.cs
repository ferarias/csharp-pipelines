using System.Collections.Generic;

namespace Pipelines.ApiTests.Dto
{
    public class ConnectorResponse
    {
        public int Id { get; set; }

        public IDictionary<int, bool> Availability { get; set; }
    }
}
