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
        /// --form 'file=@"AOhhVLaro/dummy.pdf"' \
        /// --form 'filename="dummy6"' \
        /// --form 'description="A pdf file"'
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
                string? authorizedIdString = User?.FindFirst("id")?.Value;
                if (authorizedIdString is null)
                {
                    response.Fail("Error reading authorized user id from bearer token.");
                    return StatusCode(400, response);
                }
                var authorizedId = new Guid(authorizedIdString);

                // Try to determine the content type.
                new FileExtensionContentTypeProvider()
                    .TryGetContentType(file.FileName, out var contentType);
                if (contentType is null)
                {
                    response.Fail("Unsupported file content type.");
                    return BadRequest(response);
                }

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
                        ContentType = contentType,
                        Description = metadata["description"],
                        Category = metadata["category"],
                        UserPosted = authorizedId
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
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> ReadAll()
        {
            try
            {
                var serviceRequest = new KeeperServiceRequest();
                var serviceResponse = await _keeperService.ReadAll(serviceRequest);
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

        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> ReadById(Guid id)
        {
            try
            {
                var serviceRequest = new KeeperServiceRequest(id);
                var serviceResponse = await _keeperService.ReadById(serviceRequest);
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
                        // Verify we really have the filename and binary data if we think we have achieved success.
                        if (serviceResponse.Item?.Filename is null || serviceResponse.BinaryData is not { Length: > 0 })
                        {
                            throw new Exception("Keeper service replied with succcess but filename or binary data not returned.");
                        }
                        // Try to determine the content type.
                        new FileExtensionContentTypeProvider()
                            .TryGetContentType(serviceResponse.Item.Filename, out var contentType);
                        if (contentType is null)
                        {
                            throw new Exception("Could not determine content type for a keeper from the database.");
                        }
                        // Set and return the file content result.
                        var fileContentResult = new FileContentResult(serviceResponse.BinaryData, contentType)
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
        [HttpGet("v1/keepers/{id}/permits")]
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

        #endregion READ

        #region UPDATE

        [HttpPut("")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> UpdateById([FromBody] KeeperDto keeperDto)
        {
            try
            {
                var serviceRequest = new KeeperServiceRequest(keeperDto);
                var serviceResponse = await _keeperService.UpdateById(serviceRequest);
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
