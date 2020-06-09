using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pipelines.ApiTests.Dto
{
    public class HubRequest
    {
        public int Id { get; set; }
    }

    public class HubResponse    
    {
        public int Id { get; set; }
    }

    public class ConnectorRequest
    {
        public int Id { get; set; }
    }

    public class ConnectorResponse
    {
        public int Id { get; set; }
    }

    public class ProviderRequest
    {
        public int Id { get; set; }
    }

    public class ProviderResponse
    {
        public int Id { get; set; }
    }
}
