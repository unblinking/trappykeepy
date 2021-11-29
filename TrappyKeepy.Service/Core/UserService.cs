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
            if (request.Item?.Name is null || request.Item?.Email is null || request.Item?.Password is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Name, email, password, and role are required to create a user.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, true))
            {
                try
                {
                    // Verify the requested user name is not already in use.
                    var existingNameCount = await unitOfWork.UserRepository
                        .CountByColumnValue("name", request.Item.Name);
                    if (existingNameCount > 0)
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Requested user name is already in use.";
                        return response;
                    }
                    // Verify the requested user email is not already in use.
                    var existingEmailCount = await unitOfWork.UserRepository
                        .CountByColumnValue("email", request.Item.Email);
                    if (existingEmailCount > 0)
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Requested user email is already in use.";
                        return response;
                    }

                    // Create the new user record now.
                    var id = await unitOfWork.UserRepository.Create(request.Item);

                    // Commit changes in this transaction.
                    unitOfWork.Commit();

                    // Pass a UserDto back to the controller.
                    response.Item = new UserDto() { Id = id };

                    // Success if we made it this far.
                    response.Outcome = OutcomeType.Success;
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    unitOfWork.Dispose();
                    response.Outcome = OutcomeType.Error;
                    return response;
                }
            }
            return response;
        }

        public async Task<UserServiceResponse> ReadAll(UserServiceRequest request)
        {
            var response = new UserServiceResponse();
            using (var unitOfWork = new UnitOfWork(connectionString, false))
            {
                try
                {
                    // Read the user records now.
                    var users = await unitOfWork.UserRepository.ReadAll();

                    // Pass a list of userDtos back to the controller.
                    var userDtos = new List<UserDto>();
                    foreach (var user in users)
                    {
                        var userDto = new UserDto()
                        {
                            Id = user.Id,
                            Name = user.Name,
                            // Do not include the salted/hashed password.
                            Email = user.Email,
                            Role = user.Role,
                            DateCreated = user.DateCreated,
                            DateActivated = user.DateActivated,
                            DateLastLogin = user.DateLastLogin
                        };
                        userDtos.Add(userDto);
                    }
                    response.List = userDtos;

                    // Success if we made it this far.
                    response.Outcome = OutcomeType.Success;
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    unitOfWork.Dispose();
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
                response.ErrorMessage = "User id is required to find a user.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, false))
            {
                try
                {
                    // Read the user record now.
                    var user = await unitOfWork.UserRepository.ReadById((Guid)request.Id);

                    // Pass a UserDto back to the controller.
                    response.Item = new UserDto()
                    {
                        Id = user.Id,
                        Name = user.Name,
                        // Do not include the salted/hashed password.
                        Email = user.Email,
                        Role = user.Role,
                        DateCreated = user.DateCreated,
                        DateActivated = user.DateActivated,
                        DateLastLogin = user.DateLastLogin
                    };

                    // Success if we made it this far.
                    response.Outcome = OutcomeType.Success;
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    unitOfWork.Dispose();
                    response.Outcome = OutcomeType.Error;
                    return response;
                }
            }
            return response;
        }

        public async Task<UserServiceResponse> UpdateById(UserServiceRequest request)
        {
            var response = new UserServiceResponse();
            if (request.Item is null || request.Item.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "User id is required to update a user.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, true))
            {
                try
                {
                    // Verify that the user exists.
                    var existing = await unitOfWork.UserRepository.ReadById(request.Item.Id);
                    if (existing.Id != request.Item.Id)
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Requested user id for update does not exist.";
                        return response;
                    }

                    // Update the user record now.
                    var successful = await unitOfWork.UserRepository.UpdateById(request.Item);

                    // If the user record couldn't be updated, rollback and return to the controller.
                    if (!successful)
                    {
                        unitOfWork.Rollback();
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "User was not updated.";
                        return response;
                    }

                    // Commit changes in this transaction.
                    unitOfWork.Commit();

                    response.Outcome = OutcomeType.Success;
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    unitOfWork.Dispose();
                    response.Outcome = OutcomeType.Error;
                    return response;
                }
            }
            return response;
        }

        public async Task<UserServiceResponse> UpdatePasswordById(UserServiceRequest request)
        {
            var response = new UserServiceResponse();
            if (request.Item is null || request.Item.Id == Guid.Empty || request.Item.Password is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "User id and password are required to update a user password.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, true))
            {
                try
                {
                    // Verify that the user exists.
                    var existing = await unitOfWork.UserRepository.ReadById(request.Item.Id);
                    if (existing.Id != request.Item.Id)
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Requested user or user id for update does not exist.";
                        return response;
                    }

                    // Update the user password now.
                    var successful = await unitOfWork.UserRepository.UpdatePasswordById(request.Item);

                    // If the user password couldn't be updated, rollback and return to the controller.
                    if (!successful)
                    {
                        unitOfWork.Rollback();
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "User password was not updated.";
                        return response;
                    }

                    // Commit changes in this transaction.
                    unitOfWork.Commit();

                    response.Outcome = OutcomeType.Success;
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    unitOfWork.Dispose();
                    response.Outcome = OutcomeType.Error;
                    return response;
                }
            }
            return response;
        }

        public async Task<UserServiceResponse> DeleteById(UserServiceRequest request)
        {
            var response = new UserServiceResponse();
            if (request.Id is null || request.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "User id is required to delete a user.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, true))
            {
                try
                {
                    // Verify that the user exists.
                    var existing = await unitOfWork.UserRepository.ReadById((Guid)request.Id);
                    if (existing.Id != request.Id)
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Requested user id for delete does not exist.";
                        return response;
                    }

                    // Delete the user record now.
                    var successful = await unitOfWork.UserRepository.DeleteById((Guid)request.Id);

                    // If the user record couldn't be deleted, rollback and return to the controller.
                    if (!successful)
                    {
                        unitOfWork.Rollback();
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "User was not deleted.";
                        return response;
                    }

                    // Commit changes in this transaction.
                    unitOfWork.Commit();

                    response.Outcome = OutcomeType.Success;
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    unitOfWork.Dispose();
                    response.Outcome = OutcomeType.Error;
                    return response;
                }
            }
            return response;
        }

        public async Task<UserServiceResponse> Authenticate(UserServiceRequest request)
        {
            var response = new UserServiceResponse();
            if (request.Item?.Email is null || request.Item?.Password is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "User email and password are required to authenticate a user.";
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

                    var authenticatedUser = await unitOfWork.UserRepository.Authenticate(authenticatingUser);
                    unitOfWork.Commit();

                    if (authenticatedUser.Id == Guid.Empty)
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "No match found for user email and password. Please correct the email or password and try again.";
                        return response;
                    }

                    // Create a JWT.
                    var jwtManager = new JwtManager();
                    var userRole = (UserRole)authenticatedUser.Role;
                    var jwt = jwtManager.EncodeJwt(authenticatedUser.Id, userRole, JwtType.ACCESS);
                    response.Token = jwt;

                    response.Outcome = OutcomeType.Success;
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    unitOfWork.Dispose();
                    response.Outcome = OutcomeType.Error;
                    return response;
                }
            }
            return response;
        }
    }
}
