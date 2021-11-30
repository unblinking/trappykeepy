using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Service
{
    /// <summary>
    /// The user service.
    /// A user may log in, have a role, and have group memberships. Depending on
    /// the user's role, a user may have different access to features.
    /// </summary>
    public class UserService : IUserService
    {
        /// <summary>
        /// Group database operations into a single transaction (unit of work).
        /// </summary>
        private readonly IUnitOfWork uow;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public UserService(IUnitOfWork unitOfWork)
        {
            this.uow = unitOfWork;
        }

        /// <summary>
        /// Create new user in the database.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<UserServiceResponse> Create(UserServiceRequest request)
        {
            var response = new UserServiceResponse();

            // Verify required parameters.
            if (request.Item?.Name is null || request.Item?.Email is null || request.Item?.Password is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Name, email, password, and role are required to create a user.";
                return response;
            }

            try
            {
                // Begin this transaction.
                uow.Begin();

                // Verify the requested user name is not already in use.
                var existingNameCount = await uow.users.CountByColumnValue("name", request.Item.Name);
                if (existingNameCount > 0)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Requested user name is already in use.";
                    return response;
                }
                // Verify the requested user email is not already in use.
                var existingEmailCount = await uow.users.CountByColumnValue("email", request.Item.Email);
                if (existingEmailCount > 0)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Requested user email is already in use.";
                    return response;
                }

                // Create the new user record now.
                var id = await uow.users.Create(request.Item);

                // Commit changes in this transaction.
                uow.Commit();

                // Pass a UserDto back to the controller.
                response.Item = new UserDto() { Id = id };

                // Success if we made it this far.
                response.Outcome = OutcomeType.Success;
            }
            catch (Exception)
            {
                response.Outcome = OutcomeType.Error;
            }

            return response;
        }

        /// <summary>
        /// Read all users from the database.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<UserServiceResponse> ReadAll(UserServiceRequest request)
        {
            var response = new UserServiceResponse();

            try
            {
                // Read the user records now.
                var users = await uow.users.ReadAll();

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
                response.Outcome = OutcomeType.Error;
            }

            return response;
        }

        /// <summary>
        /// Read one user from the database.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<UserServiceResponse> ReadById(UserServiceRequest request)
        {
            var response = new UserServiceResponse();

            // Verify required parameters.
            if (request.Id is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "User id is required to find a user by id.";
                return response;
            }

            try
            {
                // Read the user record now.
                var user = await uow.users.ReadById((Guid)request.Id);

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
                response.Outcome = OutcomeType.Error;
            }

            return response;
        }

        /// <summary>
        /// Update a user in the database.
        /// This method cannot be used to update a user password.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<UserServiceResponse> UpdateById(UserServiceRequest request)
        {
            var response = new UserServiceResponse();

            // Verify required parameters.
            if (request.Item is null || request.Item.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "User id is required to update a user by id.";
                return response;
            }

            try
            {
                // Begin this transaction.
                uow.Begin();

                // Verify that the user exists.
                var existing = await uow.users.ReadById(request.Item.Id);
                if (existing.Id != request.Item.Id)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Requested user id for update does not exist.";
                    return response;
                }

                // Update the user record now.
                var successful = await uow.users.UpdateById(request.Item);

                // If the user record couldn't be updated, rollback and return to the controller.
                if (!successful)
                {
                    uow.Rollback();
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "User was not updated.";
                    return response;
                }

                // Commit changes in this transaction.
                uow.Commit();

                response.Outcome = OutcomeType.Success;
            }
            catch (Exception)
            {
                response.Outcome = OutcomeType.Error;
            }

            return response;
        }

        /// <summary>
        /// Update a user password in the database.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<UserServiceResponse> UpdatePasswordById(UserServiceRequest request)
        {
            var response = new UserServiceResponse();

            // Verify required parameters.
            if (request.Item is null || request.Item.Id == Guid.Empty || request.Item.Password is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "User id and password are required to update a user password.";
                return response;
            }

            try
            {
                // Begin this transaction.
                uow.Begin();

                // Verify that the user exists.
                var existing = await uow.users.ReadById(request.Item.Id);
                if (existing.Id != request.Item.Id)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Requested user or user id for update does not exist.";
                    return response;
                }

                // Update the user password now.
                var successful = await uow.users.UpdatePasswordById(request.Item);

                // If the user password couldn't be updated, rollback and return to the controller.
                if (!successful)
                {
                    uow.Rollback();
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "User password was not updated.";
                    return response;
                }

                // Commit changes in this transaction.
                uow.Commit();

                response.Outcome = OutcomeType.Success;
            }
            catch (Exception)
            {
                response.Outcome = OutcomeType.Error;
            }

            return response;
        }

        /// <summary>
        /// Delete a user from the database.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<UserServiceResponse> DeleteById(UserServiceRequest request)
        {
            var response = new UserServiceResponse();

            // Verify required parameters.
            if (request.Id is null || request.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "User id is required to delete a user by id.";
                return response;
            }

            try
            {
                // Begin this transaction.
                uow.Begin();

                // Verify that the user exists.
                var existing = await uow.users.ReadById((Guid)request.Id);
                if (existing.Id != request.Id)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Requested user id for delete does not exist.";
                    return response;
                }

                // TODO: Figure out what to do with the keepers they are linked to.
                // TODO: Just delete those keepers? Seems wrong to do that.

                // Delete any existing user memberships first.
                var membershipsCount = await uow.memberships.CountByColumnValue("user_id", (Guid)request.Id);
                if (membershipsCount > 0)
                {
                    var successfulDeleteMemberships = await uow.memberships.DeleteByUserId((Guid)request.Id);

                    // If the user had memberships that couldn't be deleted, rollback and return to the controller.
                    if (!successfulDeleteMemberships)
                    {
                        uow.Rollback();
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "User was not deleted because existing memberships could not be deleted.";
                        return response;
                    }
                }

                // Delete the user record now.
                var successfulDeleteUser = await uow.users.DeleteById((Guid)request.Id);

                // If the user record couldn't be deleted, rollback and return to the controller.
                if (!successfulDeleteUser)
                {
                    uow.Rollback();
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "User was not deleted.";
                    return response;
                }

                // Commit changes in this transaction.
                uow.Commit();

                response.Outcome = OutcomeType.Success;
            }
            catch (Exception)
            {
                response.Outcome = OutcomeType.Error;
            }

            return response;
        }

        /// <summary>
        /// Authenticate a user, creating a session/access token.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<UserServiceResponse> Authenticate(UserServiceRequest request)
        {
            var response = new UserServiceResponse();

            // Verify required parameters.
            if (request.Item?.Email is null || request.Item?.Password is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "User email and password are required to authenticate a user.";
                return response;
            }

            try
            {
                var authenticated = await uow.users.Authenticate(request.Item);

                if (authenticated.Id == Guid.Empty)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "No match found for user email and password. Please correct the email or password and try again.";
                    return response;
                }

                // Create a JWT.
                var jwtManager = new JwtManager();
                var userRole = (UserRole)authenticated.Role;
                var jwt = jwtManager.EncodeJwt(authenticated.Id, userRole, JwtType.ACCESS);
                response.Token = jwt;

                response.Outcome = OutcomeType.Success;
            }
            catch (Exception)
            {
                response.Outcome = OutcomeType.Error;
            }

            return response;
        }
    }
}
