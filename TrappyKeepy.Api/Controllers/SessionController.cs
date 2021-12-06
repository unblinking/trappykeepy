using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Api.Controllers
{
    /// <summary>
    /// The sessions controller.
    /// Manage user sessions.
    /// </summary>
    [Route("v1/sessions")]
    [ApiController]
    [Authorize]
    public class SessionController : ControllerBase
    {
        private readonly IUserService _userService;

        public SessionController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Create a user session token.
        /// </summary>
        /// <param name="userSessionDto"></param>
        /// <example>
        /// <code>
        /// curl --location --request POST 'https://api.trappykeepy.com/v1/sessions' \
        /// --header 'Content-Type: application/json' \
        /// --data-raw '{
        ///     "email": "foo@example.com",
        ///     "password": "passwordfoo"
        /// }'
        /// </code>
        /// </example>
        /// <returns>A JSON web token to use in the Authorization header for Bearer Token type authorization.</returns>
        [HttpPost("")]
        [AllowAnonymous]
        public async Task<ActionResult> Create([FromBody] UserSessionDto userSessionDto)
        {
            try
            {
                var serviceRequest = new UserServiceRequest(userSessionDto);
                var serviceResponse = await _userService.CreateSession(serviceRequest);
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
