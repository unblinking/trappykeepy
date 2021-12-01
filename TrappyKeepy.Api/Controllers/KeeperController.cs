using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Models;
using TrappyKeepy.Service;

namespace TrappyKeepy.Api.Controllers
{
    /// <summary>
    /// The keeper controller.
    /// </summary>
    [Route("v1/keeper")]
    [ApiController]
    [Authorize]
    public class KeeperController : ControllerBase
    {
        private readonly IKeeperService keeperService;

        public KeeperController(IKeeperService keeperService)
        {
            this.keeperService = keeperService;
        }

        [HttpPost("")]
        [Authorize(Roles = "manager, admin")]
        public async Task<ActionResult> Create(IFormCollection metadata, IFormFile file)
        {
            try
            {
                var response = new ControllerResponse();

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
                // Verify the binar file data was successfully received.
                if (binaryData is not { Length: > 0 })
                {
                    response.Fail("No file data was received.");
                    return StatusCode(400, response);
                }

                // Prepare the service request.
                var serviceRequest = new KeeperServiceRequest()
                {
                    BinaryData = binaryData,
                    Item = new Keeper()
                    {
                        Filename = metadata["filename"],
                        ContentType = contentType,
                        Description = metadata["description"],
                        Category = metadata["category"],
                        UserPosted = authorizedId
                    }
                };

                // Wait for the service response.
                var serviceResponse = await keeperService.Create(serviceRequest);

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

        [HttpGet("")]
        public async Task<ActionResult> ReadAll()
        {
            try
            {
                var response = new ControllerResponse();

                // Prepare the service request.
                var serviceRequest = new KeeperServiceRequest();

                // Wait for the service response.
                var serviceResponse = await keeperService.ReadAll(serviceRequest);

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
                        response.Success(serviceResponse.List); // KeeperDto objects.
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
                var serviceRequest = new KeeperServiceRequest(id);

                // Wait for the service response.
                var serviceResponse = await keeperService.ReadById(serviceRequest);

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
                        // Verify we really have the filename and binary data if we think we have achieved success.
                        if (serviceResponse.Item?.Filename is null || serviceResponse.Item?.BinaryData is not { Length: > 0 })
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
                        var fileContentResult = new FileContentResult(serviceResponse.Item.BinaryData, contentType)
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

            // Default to error if unknown outcome from the service.
            return StatusCode(500);
        }

        [HttpPut("")]
        public async Task<ActionResult> UpdateById([FromBody] KeeperDto keeperDto)
        {
            try
            {
                var response = new ControllerResponse();

                if (keeperDto.Id is null || keeperDto.Id == Guid.Empty || (Guid)keeperDto.Id == Guid.Empty)
                {
                    response.Fail("Keeper id is required to update a keeper.");
                    return BadRequest(response);
                }

                // Prepare a keeper from the keeperDto to pass to the service.
                var keeper = new Keeper() { Id = (Guid)keeperDto.Id };
                if (keeperDto.Filename is not null) keeper.Filename = keeperDto.Filename;
                if (keeperDto.Description is not null) keeper.Description = keeperDto.Description;
                if (keeperDto.Category is not null) keeper.Category = keeperDto.Category;

                // Prepare the service request.
                var serviceRequest = new KeeperServiceRequest(keeper);

                // Wait for the service response.
                var serviceResponse = await keeperService.UpdateById(serviceRequest);

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
                        response.Success("Keeper updated.");
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
                    response.Fail("Keeper id is required to delete a keeper.");
                    return BadRequest(response);
                }

                // Prepare the service request.
                var serviceRequest = new KeeperServiceRequest(id);

                // Wait for the service response.
                var serviceResponse = await keeperService.DeleteById(serviceRequest);

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
                        response.Success("Keeper deleted.");
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
