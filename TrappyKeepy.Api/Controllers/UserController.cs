using Microsoft.AspNetCore.Mvc;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Models;
using TrappyKeepy.Service;

namespace TrappyKeepy.Api.Controllers
{
    /// <summary>
    /// The user controller.
    /// </summary>
    [Route("v1/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly JwtManager jwtManager = new JwtManager();
        private readonly Helpers help = new Helpers();

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpPost("")]
        public async Task<ActionResult> Create([FromBody] UserDto userDto)
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

                if (userDto.Name is null || userDto.Password is null || userDto.Email is null)
                {
                    response.Fail("Name, password, and email are required to create a user.");
                    return BadRequest(response);
                }

                // Prepare a user from the userDto to pass to the service.
                var user = new User()
                {
                    Name = userDto.Name,
                    Password = userDto.Password,
                    Email = userDto.Email
                };
                if (userDto.Role is not null) user.Role = (short)userDto.Role;

                // Prepare the service request.
                var serviceRequest = new UserServiceRequest(user);

                // Wait for the service response.
                var serviceResponse = await userService.Create(serviceRequest);

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
                        response.Success(serviceResponse.Item); // UserDto with new id from db insert.
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
                var serviceRequest = new UserServiceRequest();

                // Wait for the service response.
                var serviceResponse = await userService.ReadAll(serviceRequest);

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
                        response.Success(serviceResponse.List); // UserDto objects.
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
                var serviceRequest = new UserServiceRequest(id);

                // Wait for the service response.
                var serviceResponse = await userService.ReadById(serviceRequest);

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
                        response.Success(serviceResponse.Item); // UserDto object.
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
        public async Task<ActionResult> UpdateById([FromBody] UserDto userDto)
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

                if (userDto.Id is null || userDto.Id == Guid.Empty || (Guid)userDto.Id == Guid.Empty)
                {
                    response.Fail("User id is required to update a user.");
                    return BadRequest(response);
                }

                // Prepare a user from the userDto to pass to the service.
                var user = new User() { Id = (Guid)userDto.Id };
                if (userDto.Name is not null) user.Name = userDto.Name;
                if (userDto.Email is not null) user.Email = userDto.Email;

                // Determine if we are updating the user role.
                if (userDto.Role is not null)
                {
                    user.Role = (short)userDto.Role;
                }
                else
                {
                    // If we aren't updating the user role, set it to -1.
                    // The repository will only update role if role is >= 0.
                    user.Role = -1;
                }

                // Prepare the service request.
                var serviceRequest = new UserServiceRequest(user);

                // Wait for the service response.
                var serviceResponse = await userService.UpdateById(serviceRequest);

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
                        response.Success("User updated.");
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

        [HttpPut("/v1/user/password")]
        public async Task<ActionResult> UpdatePasswordById([FromBody] UserDto userDto)
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

                if (userDto.Id is null || userDto.Id == Guid.Empty || (Guid)userDto.Id == Guid.Empty || userDto.Password is null)
                {
                    response.Fail("User id and password are required to update a user pasword.");
                    return BadRequest(response);
                }

                // Prepare a user from the userDto to pass to the service.
                var user = new User()
                {
                    Id = (Guid)userDto.Id,
                    Password = userDto.Password
                };

                // Prepare the service request.
                var serviceRequest = new UserServiceRequest(user);

                // Wait for the service response.
                var serviceResponse = await userService.UpdatePasswordById(serviceRequest);

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
                        response.Success("User password updated.");
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

                if (id == Guid.Empty || (Guid)id == Guid.Empty)
                {
                    response.Fail("User id is required to delete a user.");
                    return BadRequest(response);
                }

                // Prepare the service request.
                var serviceRequest = new UserServiceRequest(id);

                // Wait for the service response.
                var serviceResponse = await userService.DeleteById(serviceRequest);

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
                        response.Success("User deleted.");
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

        // TODO: Authenticate
        [HttpPost("/v1/user/session")]
        public async Task<ActionResult> Authenticate([FromBody] UserDto userDto)
        {
            try
            {
                var response = new ControllerResponse();

                if (userDto.Email is null || userDto.Password is null)
                {
                    response.Fail("User email and password are required to authenticate a user.");
                    return BadRequest(response);
                }

                // Prepare a user from the userDto to pass to the service.
                var user = new User()
                {
                    Email = userDto.Email,
                    Password = userDto.Password
                };

                // Prepare the service request.
                var serviceRequest = new UserServiceRequest(user);

                // Wait for the service response.
                var serviceResponse = await userService.Authenticate(serviceRequest);

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
                        response.Success(serviceResponse.Token);
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
