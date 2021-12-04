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
        /// Constructor.
        /// </summary>
        /// <param name="groupService"></param>
        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

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
        /// <returns></returns>
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

        [HttpPut("")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> UpdateById([FromBody] GroupDto groupDto)
        {
            try
            {
                var serviceRequest = new GroupServiceRequest(groupDto);
                var serviceResponse = await _groupService.UpdateById(serviceRequest);
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
    }
}
