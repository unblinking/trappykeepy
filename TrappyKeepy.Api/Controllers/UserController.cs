using Microsoft.AspNetCore.Mvc;
using TrappyKeepy.Domain;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Models;


namespace TrappyKeepy.Api.Controllers
{
    /// <summary>
    /// The user controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet("")]
        public async Task<ActionResult> ReadAll()
        {
            var serviceRequest = new UserServiceRequest();
            var serviceResponse = await userService.ReadAll(serviceRequest);
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
                    response.Success(serviceResponse.List);
                    return Ok(response);
            }
            // Default to error if unknown outcome from the service.
            return StatusCode(500);
        }

        [HttpPost("")]
        public async Task<ActionResult> Create([FromBody] User user)
        {
            var serviceRequest = new UserServiceRequest(user);
            var serviceResponse = await userService.Create(serviceRequest);
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
                    response.Success(serviceResponse.Id);
                    return Ok(response);
            }
            // Default to error if unknown outcome from the service.
            return StatusCode(500);
        }
    }
}
