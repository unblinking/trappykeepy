using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Api.Controllers
{
    /// <summary>
    /// The user controller.
    /// </summary>
    [Route("v1/user")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// The user service.
        /// </summary>
        private readonly IUserService _userService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="userService"></param>
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="userDto"></param>
        /// <example>
        /// <code>
        /// curl --location --request POST 'https://localhost:7294/v1/user' \
        /// --header 'Authorization: Bearer <token>' \
        /// --header 'Content-Type: application/json' \
        /// --data-raw '{
        ///     "Name": "foo",
        ///     "Password": "passwordfoo",
        ///     "Email": "foo@example.com",
        ///     "Role": "basic"
        /// }'
        /// </code>
        /// </example>
        /// <returns>The new user object including the unique id.</returns>
        [HttpPost("")]
        public async Task<ActionResult> Create([FromBody] IUserDto userDto)
        {
            try
            {
                var serviceRequest = new UserServiceRequest(userDto);
                var serviceResponse = await _userService.Create(serviceRequest);
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
                        response.Success(serviceResponse.Item);
                        return Ok(response);
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
            return StatusCode(500);
        }

        /// <summary>
        /// Read all existing users.
        /// </summary>
        /// <example>
        /// <code>
        /// curl --location --request GET 'https://localhost:7294/v1/user' \
        /// --header 'Authorization: Bearer <token>'
        /// </code>
        /// </example>
        /// <returns>An array of all existing user objects.</returns>
        [HttpGet("")]
        public async Task<ActionResult> ReadAll()
        {
            try
            {
                var serviceRequest = new UserServiceRequest();
                var serviceResponse = await _userService.ReadAll(serviceRequest);
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
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
            return StatusCode(500);
        }

        /// <summary>
        /// Read one existing user.
        /// </summary>
        /// <param name="id"></param>
        /// <example>
        /// <code>
        // curl --location --request GET 'https://localhost:7294/v1/user/00000000-0000-0000-0000-000000000000' \
        // --header 'Authorization: Bearer <token>'
        /// </code>
        /// </example>
        /// <returns>An existing user object.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult> ReadById(Guid id)
        {
            try
            {
                var serviceRequest = new UserServiceRequest(id);
                var serviceResponse = await _userService.ReadById(serviceRequest);
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
                        response.Success(serviceResponse.Item);
                        return Ok(response);
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
            return StatusCode(500);
        }

        /// <summary>
        /// Update one existing user.
        /// </summary>
        /// <param name="userDto"></param>
        /// <example>
        /// <code>
        /// curl --location --request PUT 'https://localhost:7294/v1/user' \
        /// --header 'Authorization: Bearer <token>' \
        /// --header 'Content-Type: application/json' \
        /// --data-raw '{
        ///     "Id": "00000000-0000-0000-0000-000000000000",
        ///     "Name": "bar",
        ///     "Email": "bar@example.com",
        ///     "Role": "manager"
        /// }'
        /// </code>
        /// </example>
        /// <returns>A message if successful.</returns>
        [HttpPut("")]
        public async Task<ActionResult> UpdateById([FromBody] IUserDto userDto)
        {
            try
            {
                var serviceRequest = new UserServiceRequest(userDto);
                var serviceResponse = await _userService.UpdateById(serviceRequest);
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
                return StatusCode(500);
            }
            return StatusCode(500);
        }

        /// <summary>
        /// Update one existing user's password.
        /// </summary>
        /// <param name="userDto"></param>
        /// <example>
        /// <code>
        /// curl --location --request PUT 'https://localhost:7294/v1/user/password' \
        /// --header 'Authorization: Bearer <token>' \
        /// --header 'Content-Type: application/json' \
        /// --data-raw '{
        ///     "Id": "00000000-0000-0000-0000-000000000000",
        ///     "password": "passwordbar"
        /// }'
        /// </code>
        /// </example>
        /// <returns>A message if successful.</returns>
        [HttpPut("/v1/user/password")]
        public async Task<ActionResult> UpdatePasswordById([FromBody] IUserDto userDto)
        {
            try
            {
                var serviceRequest = new UserServiceRequest(userDto);
                var serviceResponse = await _userService.UpdatePasswordById(serviceRequest);
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
                return StatusCode(500);
            }
            return StatusCode(500);
        }

        /// <summary>
        /// Delete one existing user.
        /// </summary>
        /// <param name="id"></param>
        /// <example>
        /// <code>
        /// curl --location --request DELETE 'https://localhost:7294/v1/user/00000000-0000-0000-0000-000000000000' \
        /// --header 'Authorization: Bearer <token>'
        /// </code>
        /// </example>
        /// <returns>A message if successful.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteById(Guid id)
        {
            try
            {
                var serviceRequest = new UserServiceRequest(id);
                var serviceResponse = await _userService.DeleteById(serviceRequest);
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
                return StatusCode(500);
            }
            return StatusCode(500);
        }

        /// <summary>
        /// Create a user session token.
        /// </summary>
        /// <param name="userSessionDto"></param>
        /// <example>
        /// <code>
        /// curl --location --request POST 'https://localhost:7294/v1/user/session' \
        /// --header 'Content-Type: application/json' \
        /// --data-raw '{
        ///     "email": "foo@example.com",
        ///     "password": "passwordfoo"
        /// }'
        /// </code>
        /// </example>
        /// <returns>A JSON web token to use in the Authorization header for Bearer Token type authorization.</returns>
        [HttpPost("/v1/user/session")]
        [AllowAnonymous]
        public async Task<ActionResult> Authenticate([FromBody] IUserSessionDto userSessionDto)
        {
            try
            {
                var serviceRequest = new UserServiceRequest(userSessionDto);
                var serviceResponse = await _userService.Authenticate(serviceRequest);
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
                return StatusCode(500);
            }
            return StatusCode(500);
        }
    }
}
