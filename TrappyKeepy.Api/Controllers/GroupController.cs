using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Api.Controllers
{
    /// <summary>
    /// The group controller.
    /// </summary>
    [Route("v1/groups")]
    [ApiController]
    [Authorize]
    public class GroupController : ControllerBase
    {
        /// <summary>
        /// The group service.
        /// </summary>
        private readonly IGroupService _groupService;

        /// <summary>
        /// The permit service.
        /// </summary>
        private readonly IPermitService _permitService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="groupService"></param>
        public GroupController(IGroupService groupService, IPermitService permitService)
        {
            _groupService = groupService;
            _permitService = permitService;
        }

        #region CREATE

        /// <summary>
        /// Creates a new group.
        /// </summary>
        /// <param name="groupDto"></param>
        /// <example>
        /// <code>
        /// curl --location --request POST 'https://api.trappykeepy.com/v1/groups' \
        /// --header 'Authorization: Bearer <token>' \
        /// --header 'Content-Type: application/json' \
        /// --data-raw '{
        ///     "name": "foo",
        ///     "description": "bar"
        /// }'
        /// </code>
        /// </example>
        /// <returns>The new group object including the unique id.</returns>
        [HttpPost("")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Create([FromBody] GroupDto groupDto)
        {
            try
            {
                var serviceRequest = new GroupServiceRequest(groupDto);
                var serviceResponse = await _groupService.Create(serviceRequest);
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
        /// Read all existing groups.
        /// </summary>
        /// <example>
        /// <code>
        /// curl --location --request GET 'https://api.trappykeepy.com/v1/groups' \
        /// --header 'Authorization: Bearer <token>'
        /// </code>
        /// </example>
        /// <returns>An array of all existing group objects.</returns>
        [HttpGet("")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> ReadAll()
        {
            try
            {
                var serviceRequest = new GroupServiceRequest();
                var serviceResponse = await _groupService.ReadAll(serviceRequest);
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

            // Default to error if unknown outcome from the service.
            return StatusCode(500);
        }

        /// <summary>
        /// Read one existing group.
        /// </summary>
        /// <param name="id"></param>
        /// <example>
        /// <code>
        /// curl --location --request GET 'https://api.trappykeepy.com/v1/groups/00000000-0000-0000-0000-000000000000' \
        /// --header 'Authorization: Bearer <token>'
        /// </code>
        /// </example>
        /// <returns>An existing group object.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> ReadById(Guid id)
        {
            try
            {
                var serviceRequest = new GroupServiceRequest(id);
                var serviceResponse = await _groupService.ReadById(serviceRequest);
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
        /// Read all existing permits for a group.
        /// </summary>
        /// <param name="id"></param>
        /// <example>
        /// <code>
        /// curl --location --request GET 'https://api.trappykeepy.com/v1/groups/00000000-0000-0000-0000-000000000000/permits' \
        /// --header 'Authorization: Bearer <token>'
        /// </code>
        /// </example>
        /// <returns>An array of all existing permit objects for the specified group id.</returns>
        [HttpGet("/v1/groups/{id}/permits")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> ReadPermitsByGroupId(Guid id)
        {
            try
            {
                var serviceRequest = new PermitServiceRequest(id);
                var serviceResponse = await _permitService.ReadByGroupId(serviceRequest);
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
        /// Update one existing group.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="groupDto"></param>
        /// <example>
        /// <code>
        /// curl --location --request PUT 'https://api.trappykeepy.com/v1/groups/00000000-0000-0000-0000-000000000000' \
        /// --header 'Authorization: Bearer <token>' \
        /// --header 'Content-Type: application/json' \
        /// --data-raw '{
        ///     "id": "00000000-0000-0000-0000-000000000000",
        ///     "name": "bar",
        ///     "description": "foo"
        /// }'
        /// </code>
        /// </example>
        /// <returns></returns>
        [HttpPut("/v1/groups/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody] GroupDto groupDto)
        {
            try
            {
                // Verify the route/path parameter (id) matches the body parameter (groupDto.Id).
                if (id != groupDto.Id) return BadRequest($"Id mismatch: Route {id} ≠ Body {groupDto.Id}");

                var serviceRequest = new GroupServiceRequest(groupDto);
                var serviceResponse = await _groupService.Update(serviceRequest);
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
                        response.Success("Group updated.");
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
        /// Delete one existing group.
        /// </summary>
        /// <param name="id"></param>
        /// <example>
        /// <code>
        /// curl --location --request DELETE 'https://api.trappykeepy.com/v1/groups/00000000-0000-0000-0000-000000000000' \
        /// --header 'Authorization: Bearer <token>'
        /// </code>
        /// </example>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteById(Guid id)
        {
            try
            {
                var serviceRequest = new GroupServiceRequest(id);
                var serviceResponse = await _groupService.DeleteById(serviceRequest);
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
                        response.Success("Group deleted.");
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
