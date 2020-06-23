using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pipelines.ApiTests.Dto;

namespace Pipelines.ApiTests.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ILogger<SearchController> _logger;

        private readonly IPipelineStep<HubRequest, HubResponse> _pipeline;

        public SearchController(IPipelineStep<HubRequest, HubResponse> pipeline, ILogger<SearchController> logger)
        {
            _pipeline = pipeline;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public ActionResult<float> Get(int id)
        {
            var request = new HubRequest { Id = id, Properties = new string[] { "1", "2", "3", "4", "5", "6" } };

            _logger.LogInformation("Input '{value}' [{type}]", request, request.GetType().Name);

            var response = _pipeline.Process(request);

            _logger.LogInformation("Output '{value}' [{type}]", response, request.GetType().Name);

            return Ok(response);
        }
    }
}
