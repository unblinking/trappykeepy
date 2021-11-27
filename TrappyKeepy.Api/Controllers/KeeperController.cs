using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Net.Http.Headers;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Models;

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

        public KeeperController(IKeeperService keeperService)
        {
            this.keeperService = keeperService;
        }

        [HttpPost("")]
        public async Task<ActionResult> Create(IFormFile file)
        {
            try
            {
                // Prepare the service request.
                var token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
                var keeperDto = new KeeperDto();
                using (var ms = new MemoryStream()) // TODO: Set the capacity here?
                {
                    await file.CopyToAsync(ms);
                    keeperDto.Binarydata = ms.ToArray();
                }
                keeperDto.Filename = file.FileName;
                var serviceRequest = new KeeperServiceRequest()
                {
                    Item = keeperDto,
                    BearerToken = token
                };

                // Wait for the service response.
                var serviceResponse = await keeperService.Create(serviceRequest);

                // Prepare and send the controller response back to the client.
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
                        response.Success(serviceResponse.Item); // KeeperDto with new id from db insert.
                        return Ok(response);
                }
            }
            catch (Exception)
            {
                // TODO: Log exception somewhere?
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
                var serviceRequest = new KeeperServiceRequest(id);
                var serviceResponse = await keeperService.ReadById(serviceRequest);
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
                        new FileExtensionContentTypeProvider()
                            .TryGetContentType(serviceResponse.Item.Filename, out var contentType);
                        var fileContentResult = new FileContentResult(serviceResponse.Item.Binarydata, contentType)
                        {
                            FileDownloadName = serviceResponse.Item.Filename
                        };
                        return fileContentResult;
                }
            }
            catch (Exception)
            {
                // TODO: Log exception somewhere?
                return StatusCode(500);
            }
            // Default to error if unknown outcome from the service.
            return StatusCode(500);
        }
    }
}
