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
        /// <summary>
        /// The permit service.
        /// </summary>
        private readonly IPermitService _permitService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="permitService"></param>
        public PermitController(IPermitService permitService)
        {
            _permitService = permitService;
        }

        /// <summary>
        /// Creates a new permit.
        /// </summary>
        /// <param name="permitDto"></param>
        /// <example>
        /// <code>
        /// curl --location --request POST 'https://localhost:7294/v1/permit' \
        /// --header 'Authorization: Bearer <token>' \
        /// --header 'Content-Type: application/json' \
        /// --data-raw '{
        ///     "KeeperId": "079e0237-fa94-453d-b089-5ff7480adc32",
        ///     "UserId": "079e0237-fa94-453d-b089-5ff7480adc32"
        /// }'
        /// </code>
        /// </example>
        /// <returns>The new permit object including the unique id.</returns>
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
