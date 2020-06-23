using System.Collections.Generic;

namespace Pipelines.ApiTests.Dto
{
    public class ProviderRequest
    {
        public int RequestId { get; set; }

        public IEnumerable<string> Properties { get; set; }
    }
}
