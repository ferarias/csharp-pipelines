using System.Collections.Generic;

namespace Pipelines.ApiTests.Dto
{
    public class HubRequest
    {
        public int Id { get; set; }

        public IEnumerable<string> Properties { get; set; }

        public string HubExtraInfo { get; set; }
    }
}
