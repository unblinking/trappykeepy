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

                if (membershipDto.GroupId is null || membershipDto.GroupId == Guid.Empty || (Guid)membershipDto.GroupId == Guid.Empty)
                {
                    response.Fail("Group id is required to create a group membership.");
                    return BadRequest(response);
                }
                if (membershipDto.UserId is null || membershipDto.UserId == Guid.Empty || (Guid)membershipDto.UserId == Guid.Empty)
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
                        response.Success(serviceResponse.Item); // MembershipDto with group id after db insert.
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
