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
    public class KeeperController : ControllerBase
    {
        private readonly IKeeperService keeperService;
        private readonly JwtManager jwtManager = new JwtManager();
        private readonly Helpers help = new Helpers();

        public KeeperController(IKeeperService keeperService)
        {
            this.keeperService = keeperService;
        }

        [HttpPost("")]
        public async Task<ActionResult> Create(IFormFile file)
        {
            try
            {
                var response = new ControllerResponse();

                // Verify the requester is authorized.
                JwtPayload authorized;
                try
                {
                    authorized = jwtManager.DecodeJwt(help.ParseToken(Request.Headers));
                    if (authorized.type is not JwtType.ACCESS || authorized.role < UserRole.MANAGER)
                    {
                        response.Fail("Unauthorized. Access denied.");
                        return StatusCode(401, response);
                    }
                }
                catch
                {
                    response.Fail("Unauthorized. Access denied.");
                    return StatusCode(401, response);
                }

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
                        Filename = file.FileName,
                        UserPosted = authorized.userId
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

        [HttpGet("{id}")]
        public async Task<ActionResult> ReadById(Guid id)
        {
            try
            {
                var response = new ControllerResponse();

                // Verify the requester is authorized.
                JwtPayload authorized;
                try
                {
                    authorized = jwtManager.DecodeJwt(help.ParseToken(Request.Headers));
                    if (authorized.type is not JwtType.ACCESS || authorized.role < UserRole.MANAGER)
                    {
                        response.Fail("Unauthorized. Access denied.");
                        return StatusCode(401, response);
                    }
                }
                catch
                {
                    response.Fail("Unauthorized. Access denied.");
                    return StatusCode(401, response);
                }

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
    }
}
