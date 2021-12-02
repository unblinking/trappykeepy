using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Api.Controllers
{
    /// <summary>
    /// The membership controller.
    /// </summary>
    [Route("v1/membership")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class MembershipController : ControllerBase
    {
        private readonly IMembershipService membershipService;

        public MembershipController(IMembershipService membershipService)
        {
            this.membershipService = membershipService;
        }

        [HttpPost("")]
        public async Task<ActionResult> Create([FromBody] MembershipDto membershipDto)
        {
            try
            {
                var response = new ControllerResponse();

                if (
                    membershipDto.GroupId is null ||
                    membershipDto.GroupId == Guid.Empty ||
                    (Guid)membershipDto.GroupId == Guid.Empty
                )
                {
                    response.Fail("Group id is required to create a group membership.");
                    return BadRequest(response);
                }
                if (
                    membershipDto.UserId is null ||
                    membershipDto.UserId == Guid.Empty ||
                    (Guid)membershipDto.UserId == Guid.Empty
                )
                {
                    response.Fail("User id is required to create a group membership.");
                    return BadRequest(response);
                }

                // Prepare a membership from the membershipDto to pass to the service.
                var membership = new Membership()
                {
                    GroupId = (Guid)membershipDto.GroupId,
                    UserId = (Guid)membershipDto.UserId
                };

                // Prepare the service request.
                var serviceRequest = new MembershipServiceRequest(membership);

                // Wait for the service response.
                var serviceResponse = await membershipService.Create(serviceRequest);

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
                        response.Success(serviceResponse.Item); // MembershipDto with new id from db insert.
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
                var serviceRequest = new MembershipServiceRequest();

                // Wait for the service response.
                var serviceResponse = await membershipService.ReadAll(serviceRequest);

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
                        response.Success(serviceResponse.List); // MembershipDto objects.
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

        [HttpGet("/v1/membership/group/{id}")]
        public async Task<ActionResult> ReadByGroupId(Guid group_id)
        {
            try
            {
                var response = new ControllerResponse();

                // Prepare the service request.
                var serviceRequest = new MembershipServiceRequest(group_id);

                // Wait for the service response.
                var serviceResponse = await membershipService.ReadByGroupId(serviceRequest);

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
                        response.Success(serviceResponse.Item); // MembershipDto object.
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

        [HttpGet("/v1/membership/user/{id}")]
        public async Task<ActionResult> ReadByUserId(Guid user_id)
        {
            try
            {
                var response = new ControllerResponse();

                // Prepare the service request.
                var serviceRequest = new MembershipServiceRequest(user_id);

                // Wait for the service response.
                var serviceResponse = await membershipService.ReadByUserId(serviceRequest);

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
                        response.Success(serviceResponse.Item); // MembershipDto object.
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
                    response.Fail("Membership id is required to delete a specific user membership by id.");
                    return BadRequest(response);
                }

                // Prepare the service request.
                var serviceRequest = new MembershipServiceRequest(id);

                // Wait for the service response.
                var serviceResponse = await membershipService.DeleteByUserId(serviceRequest);

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
                        response.Success("Membership deleted.");
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

        [HttpDelete("/v1/membership/group/{id}")]
        public async Task<ActionResult> DeleteByGroupId(Guid group_id)
        {
            try
            {
                var response = new ControllerResponse();

                if (group_id == Guid.Empty || (Guid)group_id == Guid.Empty)
                {
                    response.Fail("Group id is required to delete all memberships by group id.");
                    return BadRequest(response);
                }

                // Prepare the service request.
                var serviceRequest = new MembershipServiceRequest(group_id);

                // Wait for the service response.
                var serviceResponse = await membershipService.DeleteByGroupId(serviceRequest);

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
                        response.Success("Membership deleted.");
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

        [HttpDelete("/v1/membership/user/{id}")]
        public async Task<ActionResult> DeleteByUserId(Guid user_id)
        {
            try
            {
                var response = new ControllerResponse();

                if (user_id == Guid.Empty || (Guid)user_id == Guid.Empty)
                {
                    response.Fail("User id is required to delete all memberships by user id.");
                    return BadRequest(response);
                }

                // Prepare the service request.
                var serviceRequest = new MembershipServiceRequest(user_id);

                // Wait for the service response.
                var serviceResponse = await membershipService.DeleteByUserId(serviceRequest);

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
                        response.Success("Membership deleted.");
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
