using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Api.Controllers
{
    /// <summary>
    /// The groups controller.
    /// </summary>
    [Route("v1/groups")]
    [ApiController]
    [Authorize]
    public class GroupController : ControllerBase
    {

        private readonly IGroupService _groupService;
        private readonly IMembershipService _membershipService;
        private readonly IPermitService _permitService;

        public GroupController(IGroupService groupService, IMembershipService membershipService, IPermitService permitService)
        {
            _groupService = groupService;
            _membershipService = membershipService;
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

        /// <summary>
        /// Creates a new permit for an existing group.
        /// </summary>
        /// <param name="permitDto"></param>
        /// <example>
        /// <code>
        /// curl --location --request POST 'https://api.trappykeepy.com/v1/groups/00000000-0000-0000-0000-000000000000/permits' \
        /// --header 'Authorization: Bearer <token>' \
        /// --header 'Content-Type: application/json' \
        /// --data-raw '{
        ///     "KeeperId": "00000000-0000-0000-0000-000000000000",
        ///     "GroupId": "00000000-0000-0000-0000-000000000000"
        /// }'
        /// </code>
        /// </example>
        /// <returns>The new permit object including the unique id.</returns>
        [HttpPost("/v1/groups/{id}/permits")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> CreateGroupPermit([FromRoute] Guid id, [FromBody] PermitDto permitDto)
        {
            try
            {
                // Verify the route/path parameter (id) matches the body parameter (permitDto.GroupId).
                if (id != permitDto.GroupId) return BadRequest($"Id mismatch: Route {id} ≠ Body {permitDto.GroupId}");

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
        /// This will return a complex group DTO with the usual group properties plus members and permits.
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
                        response.Success(serviceResponse.ComplexDto);
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

        /// <summary>
        /// Delete all existing memberships for one group.
        /// </summary>
        /// <param name="id"></param>
        /// <example>
        /// <code>
        /// curl --location --request DELETE 'https://api.trappykeepy.com/v1/groups/00000000-0000-0000-0000-000000000000/memberships' \
        /// --header 'Authorization: Bearer <token>'
        /// </code>
        /// </example>
        /// <returns></returns>
        [HttpDelete("/v1/groups/{id}/memberships")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteMembershipsByGroupId(Guid id)
        {
            try
            {
                var serviceRequest = new MembershipServiceRequest(id);
                var serviceResponse = await _membershipService.DeleteByGroupId(serviceRequest);
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
        /// Delete all existing permits for one group.
        /// </summary>
        /// <param name="id"></param>
        /// <example>
        /// <code>
        /// curl --location --request DELETE 'https://api.trappykeepy.com/v1/groups/00000000-0000-0000-0000-000000000000/permits' \
        /// --header 'Authorization: Bearer <token>'
        /// </code>
        /// </example>
        /// <returns></returns>
        [HttpDelete("/v1/groups/{id}/permits")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeletePermitsByGroupId(Guid id)
        {
            try
            {
                var serviceRequest = new PermitServiceRequest(id);
                var serviceResponse = await _permitService.DeleteByGroupId(serviceRequest);
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
        /// Delete one existing permits for one group.
        /// </summary>
        /// <param name="id"></param>
        /// <example>
        /// <code>
        /// curl --location --request DELETE 'https://api.trappykeepy.com/v1/groups/00000000-0000-0000-0000-000000000000/permits/00000000-0000-0000-0000-000000000000' \
        /// --header 'Authorization: Bearer <token>'
        /// </code>
        /// </example>
        /// <returns></returns>
        [HttpDelete("/v1/groups/{gid}/permits/{pid}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeletePermitByPermitId(Guid gid, Guid pid)
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
