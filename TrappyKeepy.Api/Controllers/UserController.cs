using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Api.Controllers
{
    /// <summary>
    /// The user controller.
    /// </summary>
    [Route("v1/users")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IMembershipService _membershipService;
        private readonly IPermitService _permitService;
        private readonly IUserService _userService;

        public UserController(IMembershipService membershipService, IPermitService permitService, IUserService userService)
        {
            _membershipService = membershipService;
            _permitService = permitService;
            _userService = userService;
        }

        #region CREATE

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="userDto"></param>
        /// <example>
        /// <code>
        /// curl --location --request POST 'https://api.trappykeepy.com/v1/users' \
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
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> CreateUser([FromBody] UserDto userDto)
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
        /// Create a new membership for an existing user.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="membershipDto"></param>
        /// <example>
        /// <code>
        /// curl --location --request POST 'https://api.trappykeepy.com/v1/users/00000000-0000-0000-0000-000000000000/memberships' \
        /// --header 'Authorization: Bearer <token>' \
        /// --header 'Content-Type: application/json' \
        /// --data-raw '{
        ///     "GroupId": "00000000-0000-0000-0000-000000000000",
        ///     "UserId": "00000000-0000-0000-0000-000000000000"
        /// }'
        /// </code>
        /// </example>
        /// <returns></returns>
        [HttpPost("/v1/users/{id}/memberships")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> CreateUserMembership([FromRoute] Guid id, [FromBody] MembershipDto membershipDto)
        {
            try
            {
                // Verify the route/path parameter (id) matches the body parameter (membershipDto.UserId).
                if (id != membershipDto.UserId) return BadRequest($"Id mismatch: Route {id} ≠ Body {membershipDto.UserId}");

                var serviceRequest = new MembershipServiceRequest(membershipDto);
                var serviceResponse = await _membershipService.Create(serviceRequest);
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
        /// Creates a new permit for an existing user.
        /// </summary>
        /// <param name="permitDto"></param>
        /// <example>
        /// <code>
        /// curl --location --request POST 'https://api.trappykeepy.com/v1/users/00000000-0000-0000-0000-000000000000/permits' \
        /// --header 'Authorization: Bearer <token>' \
        /// --header 'Content-Type: application/json' \
        /// --data-raw '{
        ///     "KeeperId": "00000000-0000-0000-0000-000000000000",
        ///     "UserId": "00000000-0000-0000-0000-000000000000"
        /// }'
        /// </code>
        /// </example>
        /// <returns>The new permit object including the unique id.</returns>
        [HttpPost("/v1/users/{id}/permits")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> CreateUserPermit([FromRoute] Guid id, [FromBody] PermitDto permitDto)
        {
            try
            {
                // Verify the route/path parameter (id) matches the body parameter (permitDto.UserId).
                if (id != permitDto.UserId) return BadRequest($"Id mismatch: Route {id} ≠ Body {permitDto.UserId}");

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

        #endregion CREATE

        #region READ

        /// <summary>
        /// Read all existing users.
        /// </summary>
        /// <example>
        /// <code>
        /// curl --location --request GET 'https://api.trappykeepy.com/v1/users' \
        /// --header 'Authorization: Bearer <token>'
        /// </code>
        /// </example>
        /// <returns>An array of all existing user objects.</returns>
        [HttpGet("")]
        [Authorize(Roles = "admin")]
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
        /// curl --location --request GET 'https://api.trappykeepy.com/v1/users/00000000-0000-0000-0000-000000000000' \
        /// --header 'Authorization: Bearer <token>'
        /// </code>
        /// </example>
        /// <returns>An existing user object.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
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
        /// Read all existing memberships for a user.
        /// </summary>
        /// <param name="id"></param>
        /// <example>
        /// <code>
        /// curl --location --request GET 'https://api.trappykeepy.com/v1/users/00000000-0000-0000-0000-000000000000/memberships' \
        /// --header 'Authorization: Bearer <token>'
        /// </code>
        /// </example>
        /// <returns></returns>
        [HttpGet("/v1/users/{id}/memberships")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> ReadMembershipsByUserId(Guid id)
        {
            try
            {
                var serviceRequest = new MembershipServiceRequest(id);
                var serviceResponse = await _membershipService.ReadByUserId(serviceRequest);
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
        /// Read all existing permits for a user.
        /// </summary>
        /// <param name="id"></param>
        /// <example>
        /// <code>
        /// curl --location --request GET 'https://api.trappykeepy.com/v1/users/00000000-0000-0000-0000-000000000000/permits' \
        /// --header 'Authorization: Bearer <token>'
        /// </code>
        /// </example>
        /// <returns>An array of all existing permit objects for the specified user id.</returns>
        [HttpGet("/v1/users/{id}/permits")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> ReadPermitsByUserId(Guid id)
        {
            try
            {
                var serviceRequest = new PermitServiceRequest(id);
                var serviceResponse = await _permitService.ReadByUserId(serviceRequest);
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

        #endregion READ

        #region UPDATE

        /// <summary>
        /// Update one existing user.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userDto"></param>
        /// <example>
        /// <code>
        /// curl --location --request PUT 'https://api.trappykeepy.com/v1/users/00000000-0000-0000-0000-000000000000' \
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
        /// <returns></returns>
        [HttpPut("/v1/users/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody] UserDto userDto)
        {
            try
            {
                // Verify the route/path parameter (id) matches the body parameter (userDto.Id).
                if (id != userDto.Id) return BadRequest($"Id mismatch: Route {id} ≠ Body {userDto.Id}");

                var serviceRequest = new UserServiceRequest(userDto);
                var serviceResponse = await _userService.Update(serviceRequest);
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
        /// curl --location --request PUT 'https://api.trappykeepy.com/v1/users/00000000-0000-0000-0000-000000000000/password' \
        /// --header 'Authorization: Bearer <token>' \
        /// --header 'Content-Type: application/json' \
        /// --data-raw '{
        ///     "Id": "00000000-0000-0000-0000-000000000000",
        ///     "password": "passwordbar"
        /// }'
        /// </code>
        /// </example>
        /// <returns>A message if successful.</returns>
        [HttpPut("/v1/users/{id}/password")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> UpdatePassword([FromRoute] Guid id, [FromBody] UserDto userDto)
        {
            try
            {
                // Verify the route/path parameter (id) matches the body parameter (userDto.Id).
                if (id != userDto.Id) return BadRequest($"Id mismatch: Route {id} ≠ Body {userDto.Id}");

                var serviceRequest = new UserServiceRequest(userDto);
                var serviceResponse = await _userService.UpdatePassword(serviceRequest);
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

        #endregion UPDATE

        #region DELETE

        /// <summary>
        /// Delete one existing user.
        /// </summary>
        /// <param name="id"></param>
        /// <example>
        /// <code>
        /// curl --location --request DELETE 'https://api.trappykeepy.com/v1/users/00000000-0000-0000-0000-000000000000' \
        /// --header 'Authorization: Bearer <token>'
        /// </code>
        /// </example>
        /// <returns>A message if successful.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
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
        /// Delete all existing memberships for one user.
        /// </summary>
        /// <param name="id"></param>
        /// <example>
        /// <code>
        /// curl --location --request DELETE 'https://api.trappykeepy.com/v1/users/00000000-0000-0000-0000-000000000000/memberships' \
        /// --header 'Authorization: Bearer <token>'
        /// </code>
        /// </example>
        /// <returns></returns>
        [HttpDelete("/v1/users/{id}/memberships")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteMembershipsByUserId(Guid id)
        {
            try
            {
                var serviceRequest = new MembershipServiceRequest(id);
                var serviceResponse = await _membershipService.DeleteByUserId(serviceRequest);
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
                        response.Success("Memberships deleted.");
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
        /// Delete one existing membership for one user.
        /// </summary>
        /// <param name="id"></param>
        /// <example>
        /// <code>
        /// curl --location --request DELETE 'https://api.trappykeepy.com/v1/users/00000000-0000-0000-0000-000000000000/memberships/00000000-0000-0000-0000-000000000000' \
        /// --header 'Authorization: Bearer <token>'
        /// </code>
        /// </example>
        /// <returns></returns>
        [HttpDelete("/v1/users/{uid}/memberships/{mid}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteMembershipByMembershipId(Guid uid, Guid mid)
        {
            try
            {
                var serviceRequest = new MembershipServiceRequest(mid);
                var serviceResponse = await _membershipService.DeleteById(serviceRequest);
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
                        response.Success("Membership deleted.");
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
        /// Delete all existing permits for one user.
        /// </summary>
        /// <param name="id"></param>
        /// <example>
        /// <code>
        /// curl --location --request DELETE 'https://api.trappykeepy.com/v1/users/00000000-0000-0000-0000-000000000000/permits' \
        /// --header 'Authorization: Bearer <token>'
        /// </code>
        /// </example>
        /// <returns></returns>
        [HttpDelete("/v1/users/{id}/permits")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeletePermitsByUserId(Guid id)
        {
            try
            {
                var serviceRequest = new PermitServiceRequest(id);
                var serviceResponse = await _permitService.DeleteByUserId(serviceRequest);
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
                        response.Success("Permits deleted.");
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
        /// Delete one existing permits for one user.
        /// </summary>
        /// <param name="id"></param>
        /// <example>
        /// <code>
        /// curl --location --request DELETE 'https://api.trappykeepy.com/v1/users/00000000-0000-0000-0000-000000000000/permits/00000000-0000-0000-0000-000000000000' \
        /// --header 'Authorization: Bearer <token>'
        /// </code>
        /// </example>
        /// <returns></returns>
        [HttpDelete("/v1/users/{uid}/permits/{pid}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeletePermitByPermitId(Guid uid, Guid pid)
        {
            try
            {
                var serviceRequest = new PermitServiceRequest(pid);
                var serviceResponse = await _permitService.DeleteById(serviceRequest);
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
                        response.Success("Permit deleted.");
                        return Ok(response);
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
            return StatusCode(500);
        }

        #endregion DELETE
    }
}
