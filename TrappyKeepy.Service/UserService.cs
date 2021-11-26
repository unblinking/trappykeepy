using TrappyKeepy.Data;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Service
{
    public class UserService : IUserService
    {
        // TODO: Figure out some way to inject UnitOfWork here for testing.
        // TODO: This connection string env var is ugly too.

        private string connectionString;

        public UserService()
        {
            this.connectionString = $"{Environment.GetEnvironmentVariable("TKDB_CONN_STRING")}";
        }

        public UserService(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<UserServiceResponse> Create(UserServiceRequest request)
        {
            var response = new UserServiceResponse();
            if (request.Item is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Requested new user was not defined.";
                return response;
            }
            if (request.Item.Name is null || request.Item.Email is null || request.Item.Password is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Name, Email, and Password are required to create a user.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, true))
            {
                try
                {
                    // TODO: Verify requesting user has permission to make this request.
                    // Received a UserDto from the controller. Turn that into a User.
                    var newUser = new User()
                    {
                        Name = request.Item.Name,
                        Password = request.Item.Password, // Plaintext password here from the request Dto.
                        Email = request.Item.Email
                    };
                    var existingNameCount = await unitOfWork.UserRepository
                        .CountByColumnValue("name", newUser.Name);
                    if (existingNameCount > 0)
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Requested new user name already in use.";
                        return response;
                    }
                    var existingEmailCount = await unitOfWork.UserRepository
                        .CountByColumnValue("email", newUser.Email);
                    if (existingEmailCount > 0)
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Requested new user email already in use.";
                        return response;
                    }
                    var newId = await unitOfWork.UserRepository.Create(newUser);
                    unitOfWork.Commit();

                    // Pass a UserDto back to the controller.
                    response.Item = new UserDto()
                    {
                        Id = newId // Id from the database insert.
                    };
                    response.Outcome = OutcomeType.Success;
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    unitOfWork.Dispose();
                    // TODO: Log exception somewhere?
                    response.Outcome = OutcomeType.Error;
                    return response;
                }
            }
            return response;
        }

        public async Task<UserServiceResponse> ReadAll(UserServiceRequest request)
        {
            var response = new UserServiceResponse();
            using (var unitOfWork = new UnitOfWork(connectionString, true))
            {
                try
                {
                    // TODO: Verify requesting user has permission to make this request.
                    var userList = await unitOfWork.UserRepository.ReadAll();
                    unitOfWork.Commit();

                    // Pass userDto objects back to the controller.
                    var userDtos = new List<UserDto>();
                    foreach (User user in userList)
                    {
                        var userDto = new UserDto()
                        {
                            Id = user.Id,
                            Name = user.Name,
                            // Do not include the salted/hashed password.
                            Email = user.Email,
                            DateCreated = user.DateCreated,
                            DateActivated = user.DateActivated,
                            DateLastLogin = user.DateLastLogin
                        };
                        userDtos.Add(userDto);
                    }
                    response.List = userDtos;
                    response.Outcome = OutcomeType.Success;
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    unitOfWork.Dispose();
                    // TODO: Log exception somewhere?
                    response.Outcome = OutcomeType.Error;
                    return response;
                }
            }
            return response;
        }

        public async Task<UserServiceResponse> ReadById(UserServiceRequest request)
        {
            var response = new UserServiceResponse();
            if (request.Id is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Requested user Id was not defined.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, true))
            {
                try
                {
                    // TODO: Verify requesting user has permission to make this request.
                    var user = await unitOfWork.UserRepository.ReadById((Guid)request.Id);
                    unitOfWork.Commit();

                    // TODO: Get User objects from unitOfWork, convert to Dto objects for the service response.
                    // TODO: Don't include the password in the Dto.


                    var userDto = new UserDto()
                    {
                        Id = user.Id,
                        Name = user.Name,
                        // Do not include the salted/hashed password.
                        Email = user.Email,
                        DateCreated = user.DateCreated,
                        DateActivated = user.DateActivated,
                        DateLastLogin = user.DateLastLogin
                    };
                    response.Item = userDto;
                    response.Outcome = OutcomeType.Success;
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    unitOfWork.Dispose();
                    // TODO: Log exception somewhere?
                    response.Outcome = OutcomeType.Error;
                    return response;
                }
            }
            return response;
        }

        public async Task<UserServiceResponse> UpdateById(UserServiceRequest request)
        {
            var response = new UserServiceResponse();
            if (request.Item is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Requested user for update was not defined.";
                return response;
            }
            if (request.Item.Id is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Requested user id for update was not defined.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, true))
            {
                try
                {
                    // TODO: Verify requesting user has permission to make this request.
                    // TODO: Verify that the user exists first?
                    // Received a UserDto from the controller. Turn that into a User.
                    var dto = request.Item;
                    var updatee = new User();
                    if (dto.Id is not null) updatee.Id = (Guid)dto.Id;
                    if (dto.Name is not null) updatee.Name = dto.Name;
                    if (dto.Email is not null) updatee.Email = dto.Email;
                    if (dto.DateActivated is not null) updatee.DateActivated = dto.DateActivated;
                    if (dto.DateLastLogin is not null) updatee.DateLastLogin = dto.DateLastLogin;

                    // The updater updates the updatee.
                    var successful = await unitOfWork.UserRepository.UpdateById(updatee);
                    unitOfWork.Commit();
                    if (successful)
                    {
                        response.Outcome = OutcomeType.Success;
                    }
                    else
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "User was not updated.";
                    }
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    unitOfWork.Dispose();
                    // TODO: Log exception somewhere?
                    response.Outcome = OutcomeType.Error;
                    return response;
                }
            }
            return response;
        }

        public async Task<UserServiceResponse> UpdatePasswordById(UserServiceRequest request)
        {
            var response = new UserServiceResponse();
            if (request.Item is null || request.Item.Id is null || request.Item.Password is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Required fields were not defined. Please include user id and password and try again.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, true))
            {
                try
                {
                    // TODO: Verify requesting user has permission to make this request.
                    // TODO: Verify that the user exists first?
                    // Received a UserDto from the controller. Turn that into a User.
                    var dto = request.Item;
                    var updatee = new User();
                    if (dto.Id is not null) updatee.Id = (Guid)dto.Id;
                    if (dto.Password is not null) updatee.Password = dto.Password;

                    // The updater updates the updatee.
                    var successful = await unitOfWork.UserRepository.UpdatePasswordById(updatee);
                    unitOfWork.Commit();
                    if (successful)
                    {
                        response.Outcome = OutcomeType.Success;
                    }
                    else
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "User password was not updated.";
                    }
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    unitOfWork.Dispose();
                    // TODO: Log exception somewhere?
                    response.Outcome = OutcomeType.Error;
                    return response;
                }
            }
            return response;
        }

        public async Task<UserServiceResponse> DeleteById(UserServiceRequest request)
        {
            var response = new UserServiceResponse();
            if (request.Id is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Requested user Id was not defined.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, true))
            {
                try
                {
                    // TODO: Verify requesting user has permission to make this request.
                    // TODO: Verify that the user exists first?
                    var successful = await unitOfWork.UserRepository.DeleteById((Guid)request.Id);
                    unitOfWork.Commit();
                    if (successful)
                    {
                        response.Outcome = OutcomeType.Success;
                    }
                    else
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "User was not deleted.";
                    }
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    unitOfWork.Dispose();
                    // TODO: Log exception somewhere?
                    response.Outcome = OutcomeType.Error;
                    return response;
                }
            }
            return response;
        }

        public async Task<UserServiceResponse> Authenticate(UserServiceRequest request)
        {
            var response = new UserServiceResponse();
            if (request.Item is null || request.Item.Email is null || request.Item.Password is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Requested user for authentication was not defined. Please provide a user email and password and try again.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, true))
            {
                try
                {
                    // Received a UserDto from the controller. Turn that into a User.
                    var authenticatingUser = new User()
                    {
                        Password = request.Item.Password, // Plaintext password here from the request Dto.
                        Email = request.Item.Email,
                    };

                    var authenticatedId = await unitOfWork.UserRepository.Authenticate(authenticatingUser);
                    unitOfWork.Commit();

                    if (authenticatedId == Guid.Empty)
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "No match found for email and password. Please correct the email or password and try again.";
                        return response;
                    }

                    // Create a JWT with encrypted payload values.
                    var jwtService = new JwtService();
                    var jwt = jwtService.EncodeJwt(authenticatedId, JwtTokenType.ACCESS);

                    response.Token = jwt;
                    response.Outcome = OutcomeType.Success;
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    unitOfWork.Dispose();
                    // TODO: Log exception somewhere?
                    response.Outcome = OutcomeType.Error;
                    return response;
                }
            }
            return response;
        }
    }
}