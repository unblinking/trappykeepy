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

        [HttpPost("")]
        public async Task<ActionResult> Create([FromBody] UserDto userDto)
        {
            try
            {
                var serviceRequest = new UserServiceRequest(userDto);
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
                        response.Success(serviceResponse.Item); // UserDto with new id from db insert.
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

        [HttpGet("")]
        public async Task<ActionResult> ReadAll()
        {
            try
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
                        response.Success(serviceResponse.List); // UserDto objects.
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

        [HttpGet("{id}")]
        public async Task<ActionResult> ReadById(Guid id)
        {
            try
            {
                var serviceRequest = new UserServiceRequest(id);
                var serviceResponse = await userService.ReadById(serviceRequest);
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
                        response.Success(serviceResponse.Item); // UserDto object.
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

        // TODO: UpdateById
        [HttpPut("")]
        public async Task<ActionResult> UpdateById([FromBody] UserDto userDto)
        {
            try
            {
                var serviceRequest = new UserServiceRequest(userDto);
                var serviceResponse = await userService.UpdateById(serviceRequest);
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
                        response.Success("User updated.");
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

        [HttpPut("/password")]
        public async Task<ActionResult> UpdatePasswordById([FromBody] UserDto userDto)
        {
            try
            {
                var serviceRequest = new UserServiceRequest(userDto);
                var serviceResponse = await userService.UpdatePasswordById(serviceRequest);
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
                        response.Success("User password updated.");
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

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteById(Guid id)
        {
            try
            {
                var serviceRequest = new UserServiceRequest(id);
                var serviceResponse = await userService.DeleteById(serviceRequest);
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
                        response.Success("User deleted.");
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

        // TODO: Authenticate
        [HttpPost("/authentication")]
        public async Task<ActionResult> Authenticate([FromBody] UserDto userDto)
        {
            try
            {
                var serviceRequest = new UserServiceRequest(userDto);
                var serviceResponse = await userService.Authenticate(serviceRequest);
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
                        response.Success(serviceResponse.Token);
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
