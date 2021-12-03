using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Api.Controllers
{
    /// <summary>
    /// The permit controller.
    /// </summary>
    [Route("v1/permit")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class PermitController : ControllerBase
    {
        private readonly IPermitService _permitService;

        public PermitController(IPermitService permitService)
        {
            _permitService = permitService;
        }

        [HttpPost("")]
        public async Task<ActionResult> Create([FromBody] PermitDto permitDto)
        {
            try
            {
                var serviceRequest = new PermitServiceRequest(permitDto);
                var serviceResponse = await _permitService.Create(serviceRequest);
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
                        response.Success(serviceResponse.Item); // permitDto with new id from db insert.
                        return Ok(response);
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
            return StatusCode(500);
        }

    }
}
