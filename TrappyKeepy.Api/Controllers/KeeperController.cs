using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Api.Controllers
{
    /// <summary>
    /// The keepers controller.
    /// </summary>
    [Route("v1/keepers")]
    [ApiController]
    [Authorize]
    public class KeeperController : ControllerBase
    {
        private readonly IKeeperService _keeperService;
        private readonly IPermitService _permitService;

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
                var serviceRequest = new KeeperServiceRequest(metadata, file, User);
                var serviceResponse = await _keeperService.Create(serviceRequest);
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
                var serviceRequest = new KeeperServiceRequest() { PrincipalUser = User };
                var serviceResponse = await _keeperService.ReadAllPermitted(serviceRequest);
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
                var serviceRequest = new KeeperServiceRequest() { Id = id, PrincipalUser = User };
                var serviceResponse = await _keeperService.ReadByIdPermitted(serviceRequest);
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
                        if (serviceResponse.FileContentResult is not null) return serviceResponse.FileContentResult;
                        else return StatusCode(500, response);
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

        /// <summary>
        /// Delete all existing permits for one keeper.
        /// </summary>
        /// <param name="id"></param>
        /// <example>
        /// <code>
        /// curl --location --request DELETE 'https://api.trappykeepy.com/v1/keepers/00000000-0000-0000-0000-000000000000/permits' \
        /// --header 'Authorization: Bearer <token>'
        /// </code>
        /// </example>
        /// <returns></returns>
        [HttpDelete("/v1/keepers/{id}/permits")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeletePermitsByKeeperId(Guid id)
        {
            try
            {
                var serviceRequest = new PermitServiceRequest(id);
                var serviceResponse = await _permitService.DeleteByKeeperId(serviceRequest);
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

        #endregion DELETE
    }
}
