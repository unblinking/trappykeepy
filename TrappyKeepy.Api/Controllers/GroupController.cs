using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Api.Controllers
{
    /// <summary>
    /// The group controller.
    /// </summary>
    [Route("v1/group")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService groupService;

        public GroupController(IGroupService groupService)
        {
            this.groupService = groupService;
        }

        [HttpPost("")]
        public async Task<ActionResult> Create([FromBody] GroupDto groupDto)
        {
            try
            {
                var response = new ControllerResponse();

                if (groupDto.Name is null)
                {
                    response.Fail("Name is required to create a group.");
                    return BadRequest(response);
                }

                // Prepare a group from the groupDto to pass to the service.
                var group = new Group()
                {
                    Name = groupDto.Name,
                    Description = groupDto.Description
                };

                // Prepare the service request.
                var serviceRequest = new GroupServiceRequest(group);

                // Wait for the service response.
                var serviceResponse = await groupService.Create(serviceRequest);

                // Send the controller response back to the client.
                switch (serviceResponse.Outcome)
                {
                    case OutcomeType.Error:
                        response.Error();
                        return StatusCode(500, response);
                    case OutcomeType.Fail:
                        response.Fail(serviceResponse.ErrorMessage);
                        return BadRequest(response);
                    case OutcomeType.Success:
                        response.Success(serviceResponse.Item); // GroupDto with new id from db insert.
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

        [HttpGet("")]
        public async Task<ActionResult> ReadAll()
        {
            try
            {
                var response = new ControllerResponse();

                // Prepare the service request.
                var serviceRequest = new GroupServiceRequest();

                // Wait for the service response.
                var serviceResponse = await groupService.ReadAll(serviceRequest);

                // Send the controller response back to the client.
                switch (serviceResponse.Outcome)
                {
                    case OutcomeType.Error:
                        response.Error();
                        return StatusCode(500, response);
                    case OutcomeType.Fail:
                        response.Fail(serviceResponse.ErrorMessage);
                        return BadRequest(response);
                    case OutcomeType.Success:
                        response.Success(serviceResponse.List); // GroupDto objects.
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
        public async Task<ActionResult> ReadById(Guid id)
        {
            try
            {
                var response = new ControllerResponse();

                // Prepare the service request.
                var serviceRequest = new GroupServiceRequest(id);

                // Wait for the service response.
                var serviceResponse = await groupService.ReadById(serviceRequest);

                // Send the controller response back to the client.
                switch (serviceResponse.Outcome)
                {
                    case OutcomeType.Error:
                        response.Error();
                        return StatusCode(500, response);
                    case OutcomeType.Fail:
                        response.Fail(serviceResponse.ErrorMessage);
                        return BadRequest(response);
                    case OutcomeType.Success:
                        response.Success(serviceResponse.Item); // GroupDto object.
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

        [HttpPut("")]
        public async Task<ActionResult> UpdateById([FromBody] GroupDto groupDto)
        {
            try
            {
                var response = new ControllerResponse();

                if (groupDto.Id is null || groupDto.Id == Guid.Empty || (Guid)groupDto.Id == Guid.Empty)
                {
                    response.Fail("Group id is required to update a group by id.");
                    return BadRequest(response);
                }

                // Prepare a group from the groupDto to pass to the service.
                var group = new Group() { Id = (Guid)groupDto.Id };
                if (groupDto.Name is not null) group.Name = groupDto.Name;
                if (groupDto.Description is not null) group.Description = groupDto.Description;

                // Prepare the service request.
                var serviceRequest = new GroupServiceRequest(group);

                // Wait for the service response.
                var serviceResponse = await groupService.UpdateById(serviceRequest);

                // Send the controller response back to the client.
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

            // Default to error if unknown outcome from the service.
            return StatusCode(500);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteById(Guid id)
        {
            try
            {
                var response = new ControllerResponse();

                if (id == Guid.Empty || (Guid)id == Guid.Empty)
                {
                    response.Fail("Group id is required to delete a group by id.");
                    return BadRequest(response);
                }

                // Prepare the service request.
                var serviceRequest = new GroupServiceRequest(id);

                // Wait for the service response.
                var serviceResponse = await groupService.DeleteById(serviceRequest);

                // Send the controller response back to the client.
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

            // Default to error if unknown outcome from the service.
            return StatusCode(500);
        }

    }
}
