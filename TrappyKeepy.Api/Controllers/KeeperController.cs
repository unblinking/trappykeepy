using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Api.Controllers
{
    /// <summary>
    /// The keeper controller.
    /// </summary>
    [Route("v1/keepers")]
    [ApiController]
    [Authorize]
    public class KeeperController : ControllerBase
    {
        /// <summary>
        /// The keeper service.
        /// </summary>
        private readonly IKeeperService _keeperService;

        /// <summary>
        /// The permit service.
        /// </summary>
        private readonly IPermitService _permitService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="keeperService"></param>
        public KeeperController(IKeeperService keeperService, IPermitService permitService)
        {
            _keeperService = keeperService;
            _permitService = permitService;
        }

        #region CREATE

        /// <summary>
        /// Creates a new keeper.
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="file"></param>
        /// <example>
        /// <code>
        /// curl --location --request POST 'https://api.trappykeepy.com/v1/keepers' \
        /// --header 'Authorization: Bearer <token>' \
        /// --form 'file=@"dummy.pdf"' \
        /// --form 'filename="dummy.pdf"' \
        /// --form 'description="A pdf file"' \
        /// --form 'category="Comedy"'
        /// </code>
        /// </example>
        /// <returns></returns>
        [HttpPost("")]
        [Authorize(Roles = "manager, admin")]
        public async Task<ActionResult> Create(IFormCollection metadata, IFormFile file)
        {
            try
            {
                var response = new ControllerResponse();

                // Determine the id of the user from their authorization token.
                string? userPostedString = User?.FindFirst("id")?.Value;
                if (userPostedString is null)
                {
                    response.Fail("Error reading authorized user id from bearer token.");
                    return StatusCode(400, response);
                }
                var userPosted = new Guid(userPostedString);

                // Read the binary file data.
                byte[] binaryData;
                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    binaryData = ms.ToArray();
                }
                // Verify the binary file data was successfully received.
                if (binaryData is not { Length: > 0 })
                {
                    response.Fail("No file data was received.");
                    return StatusCode(400, response);
                }

                var filename = (string)metadata["filename"];
                if (filename is null)
                {
                    response.Fail("Filename is required to create a keeper.");
                    return StatusCode(400, response);
                }

                // Prepare the service request.
                var serviceRequest = new KeeperServiceRequest()
                {
                    BinaryData = binaryData,
                    Item = new KeeperDto()
                    {
                        Filename = filename,
                        ContentType = file.ContentType,
                        Description = metadata["description"],
                        Category = metadata["category"],
                        UserPosted = userPosted
                    }
                };

                // Wait for the service response.
                var serviceResponse = await _keeperService.Create(serviceRequest);

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
                        response.Success(serviceResponse.Item); // KeeperDto with new id from db insert.
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

        #endregion CREATE

        #region READ

        /// <summary>
        /// Read all existing keepers.
        /// </summary>
        /// <example>
        /// <code>
        /// curl --location --request GET 'https://api.trappykeepy.com/v1/keepers' \
        /// --header 'Authorization: Bearer <token>'
        /// </code>
        /// </example>
        /// <returns></returns>
        [HttpGet("")]
        [Authorize]
        public async Task<ActionResult> ReadAll()
        {
            try
            {
                var response = new ControllerResponse();

                if (User is null)
                {
                    response.Fail("Error reading authorized user from bearer token.");
                    return StatusCode(400, response);
                }

                // Determine the id of the user from their authorization token.
                string? userIdString = User.FindFirst("id")?.Value;
                if (userIdString is null)
                {
                    response.Fail("Error reading authorized user id from bearer token.");
                    return StatusCode(400, response);
                }
                var userId = new Guid(userIdString);

                var isAdmin = false;
                isAdmin = User.IsInRole("admin");

                var serviceRequest = new KeeperServiceRequest() { RequestingUserId = userId };

                IKeeperServiceResponse serviceResponse = new KeeperServiceResponse();
                if (isAdmin)
                {
                    serviceResponse = await _keeperService.ReadAll(serviceRequest);
                }
                else
                {
                    serviceResponse = await _keeperService.ReadAllPermitted(serviceRequest);
                }

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
        /// Download the binary file data for one existing keeper record.
        /// </summary>
        /// <param name="id"></param>
        /// <example>
        /// <code>
        /// curl --location --request GET 'https://api.trappykeepy.com/v1/keepers/00000000-0000-0000-0000-000000000000' \
        /// --header 'Authorization: Bearer <token>'
        /// </code>
        /// </example>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult> ReadById(Guid id)
        {
            try
            {
                var response = new ControllerResponse();

                if (User is null)
                {
                    response.Fail("Error reading authorized user from bearer token.");
                    return StatusCode(400, response);
                }

                // Determine the id of the user from their authorization token.
                string? userIdString = User.FindFirst("id")?.Value;
                if (userIdString is null)
                {
                    response.Fail("Error reading authorized user id from bearer token.");
                    return StatusCode(400, response);
                }
                var userId = new Guid(userIdString);

                var isAdmin = false;
                isAdmin = User.IsInRole("admin");

                var serviceRequest = new KeeperServiceRequest()
                {
                    Id = id,
                    RequestingUserId = userId
                };

                IKeeperServiceResponse serviceResponse = new KeeperServiceResponse();
                if (isAdmin)
                {
                    serviceResponse = await _keeperService.ReadById(serviceRequest);
                }
                else
                {
                    serviceResponse = await _keeperService.ReadByIdPermitted(serviceRequest);
                }

                switch (serviceResponse.Outcome)
                {
                    case OutcomeType.Error:
                        response.Error();
                        return StatusCode(500, response);
                    case OutcomeType.Fail:
                        response.Fail(serviceResponse.ErrorMessage);
                        return BadRequest(response);
                    case OutcomeType.Success:
                        // Verify we really have the filename and binary data if we think we have achieved success.
                        if (
                            serviceResponse.Item?.Filename is null || serviceResponse.Item.ContentType is null ||
                            serviceResponse.BinaryData is not { Length: > 0 }
                        )
                        {
                            throw new Exception("Keeper service replied with succcess but filename or binary data not returned.");
                        }
                        // Set and return the file content result.
                        var fileContentResult = new FileContentResult(serviceResponse.BinaryData, serviceResponse.Item.ContentType)
                        {
                            FileDownloadName = serviceResponse.Item.Filename
                        };
                        return fileContentResult;
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
            return StatusCode(500);
        }

        /// <summary>
        /// Read all existing permits for a keeper.
        /// </summary>
        /// <param name="id"></param>
        /// <example>
        /// <code>
        /// curl --location --request GET 'https://api.trappykeepy.com/v1/keepers/00000000-0000-0000-0000-000000000000/permits' \
        /// --header 'Authorization: Bearer <token>'
        /// </code>
        /// </example>
        /// <returns>An array of all existing permit objects for the specified keeper id.</returns>
        [HttpGet("/v1/keepers/{id}/permits")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> ReadPermitsByKeeperId(Guid id)
        {
            try
            {
                var serviceRequest = new PermitServiceRequest(id);
                var serviceResponse = await _permitService.ReadByKeeperId(serviceRequest);
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
        /// Update one existing keeper.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="keeperDto"></param>
        /// <example>
        /// <code>
        /// curl --location --request PUT 'https://api.trappykeepy.com/v1/keepers/00000000-0000-0000-0000-000000000000' \
        /// --header 'Authorization: Bearer <token>' \
        /// --header 'Content-Type: application/json' \
        /// --data-raw '{
        ///     "id": "00000000-0000-0000-0000-000000000000",
        ///     "category": "drama"
        /// }'
        /// </code>
        /// </example>
        /// <returns></returns>
        [HttpPut("/v1/keepers/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody] KeeperDto keeperDto)
        {
            try
            {
                // Verify the route/path parameter (id) matches the body parameter (keeperDto.Id).
                if (id != keeperDto.Id) return BadRequest($"Id mismatch: Route {id} ≠ Body {keeperDto.Id}");

                var serviceRequest = new KeeperServiceRequest(keeperDto);
                var serviceResponse = await _keeperService.Update(serviceRequest);
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
                        response.Success("Keeper updated.");
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
        /// Delete one existing keeper.
        /// </summary>
        /// <param name="id"></param>
        /// <example>
        /// <code>
        /// curl --location --request DELETE 'https://api.trappykeepy.com/v1/keepers/00000000-0000-0000-0000-000000000000' \
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
                var serviceRequest = new KeeperServiceRequest(id);
                var serviceResponse = await _keeperService.DeleteById(serviceRequest);
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
                        response.Success("Keeper deleted.");
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
