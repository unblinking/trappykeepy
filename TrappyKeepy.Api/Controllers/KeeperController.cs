using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Api.Controllers
{
    /// <summary>
    /// The keeper controller.
    /// </summary>
    [Route("v1/keeper")]
    [ApiController]
    public class KeeperController : ControllerBase
    {
        private readonly IKeeperService keeperService;

        public KeeperController(IKeeperService keeperService)
        {
            this.keeperService = keeperService;
        }

        [HttpPost("")]
        public async Task<ActionResult> Create([FromBody] KeeperDto keeperDto)
        {
            try
            {
                var serviceRequest = new KeeperServiceRequest(keeperDto);
                serviceRequest.BearerToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
                var serviceResponse = await keeperService.Create(serviceRequest);
                var response = new ControllerResponse();
                switch (serviceResponse.Outcome)
                {
                    case OutcomeType.Error:
                        response.Error();
                        return StatusCode(500, response);
                    case OutcomeType.Fail:
                        response.Fail(serviceResponse.ErrorMessage);
                        return BadRequest(response);
                    case OutcomeType.Success:
                        response.Success(serviceResponse.Item); // KeeperDto with new id from db insert.
                        return Ok(response);
                }
            }
            catch (Exception)
            {
                // TODO: Log exception somewhere?
                return StatusCode(500);
            }
            // Default to error if unknown outcome from the service.
            return StatusCode(500);
        }
    }
}
