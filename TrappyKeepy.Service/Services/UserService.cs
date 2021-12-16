using AutoMapper;
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
        private readonly IUnitOfWork _uow;

        /// <summary>
        /// Encode JSON web tokens.
        /// </summary>
        private readonly ITokenService _jwt;

        /// <summary>
        /// Automapper http://automapper.org/
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public UserService(IUnitOfWork unitOfWork, ITokenService tokenService, IMapper mapper)
        {
            _uow = unitOfWork;
            _jwt = tokenService;
            _mapper = mapper;
        }

        /// <summary>
        /// Create new user in the database.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IUserServiceResponse> Create(IUserServiceRequest request)
        {
            var response = new UserServiceResponse();

            // Verify required parameters.
            if (
                request.Item?.Name is null || string.IsNullOrWhiteSpace(request.Item.Name) ||
                request.Item?.Email is null || string.IsNullOrWhiteSpace(request.Item.Email) ||
                request.Item?.Password is null || string.IsNullOrWhiteSpace(request.Item.Password))
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Name (TEXT), Email (TEXT), Password (TEXT), and Role (basic, manager, or admin) are required to create a user.";
                return response;
            }

            try
            {
                // Map the controller's DTO to a domain object for the repository.
                var user = _mapper.Map<User>(request.Item);

                // Begin this transaction.
                _uow.Begin();

                // Verify the requested user name is not already in use.
                var existingNameCount = await _uow.users.CountByColumnValue("name", request.Item.Name);
                if (existingNameCount > 0)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Requested user name is already in use.";
                    return response;
                }
                // Verify the requested user email is not already in use.
                var existingEmailCount = await _uow.users.CountByColumnValue("email", request.Item.Email);
                if (existingEmailCount > 0)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Requested user email is already in use.";
                    return response;
                }

                // Create the new user record now.
                var newUser = await _uow.users.Create(user);

                // Commit changes in this transaction.
                _uow.Commit();

                // Map the repository's domain object to a DTO for the response to the controller.
                response.Item = _mapper.Map<UserDto>(newUser);

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
        public async Task<IUserServiceResponse> ReadAll(IUserServiceRequest request)
        {
            var response = new UserServiceResponse();

            try
            {
                // Read the user records now.
                var users = await _uow.users.ReadAll();

                // Map the repository's domain objects to DTOs for the response to the controller.
                var userDtos = new List<IUserDto>();
                foreach (var user in users) userDtos.Add(_mapper.Map<UserDto>(user));
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
        public async Task<IUserServiceResponse> ReadById(IUserServiceRequest request)
        {
            var response = new UserServiceResponse();

            // Verify required parameters.
            if (request.Id is null || request.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Id (UUID) is required to find a user by user id.";
                return response;
            }

            try
            {
                // Read the user record now.
                var user = await _uow.users.ReadById((Guid)request.Id);

                // Verify the user was found.
                if (user.Id != request.Id)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "User was not found with the specified id.";
                    return response;
                }

                // Read the user's memberships.
                var memberships = await _uow.memberships.ReadByUserId(user.Id);
                if (memberships.Count() > 0)
                {
                    user.Memberships = memberships;
                }

                // Read the user's permits.
                var permits = await _uow.permits.ReadByUserId(user.Id);
                if (permits.Count() > 0)
                {
                    user.Permits = permits;
                }

                // Map the repository's domain object to a DTO for the response to the controller.
                response.ComplexDto = _mapper.Map<UserComplexDto>(user);

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
        public async Task<IUserServiceResponse> Update(IUserServiceRequest request)
        {
            var response = new UserServiceResponse();

            // Verify required parameters.
            if (request.Item?.Id is null || request.Item.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Id (UUID) is required to update a user.";
                return response;
            }

            try
            {
                // Map the controller's DTO to a domain object for the repository.
                var user = _mapper.Map<User>(request.Item);

                // Begin this transaction.
                _uow.Begin();

                // Verify that the user exists.
                var existing = await _uow.users.ReadById(user.Id);
                if (existing.Id != user.Id)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Requested user id for update does not exist.";
                    return response;
                }

                // Update the user record now.
                var successful = await _uow.users.UpdateById(user);

                // If the user record couldn't be updated, rollback and return to the controller.
                if (!successful)
                {
                    _uow.Rollback();
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "User was not updated.";
                    return response;
                }

                // Commit changes in this transaction.
                _uow.Commit();

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
        public async Task<IUserServiceResponse> UpdatePassword(IUserServiceRequest request)
        {
            var response = new UserServiceResponse();

            // Verify required parameters.
            if (request.Item?.Id is null || request.Item?.Id == Guid.Empty || request.Item?.Password is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Id (UUID) and Password (TEXT) are required to update a user password.";
                return response;
            }

            try
            {
                // Map the controller's DTO to a domain object for the repository.
                var user = _mapper.Map<User>(request.Item);

                // Begin this transaction.
                _uow.Begin();

                // Verify that the user exists.
                var existing = await _uow.users.ReadById(user.Id);
                if (existing.Id != user.Id)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Requested user or user id for update does not exist.";
                    return response;
                }

                // Update the user password now.
                var successful = await _uow.users.UpdatePasswordById(user);

                // If the user password couldn't be updated, rollback and return to the controller.
                if (!successful)
                {
                    _uow.Rollback();
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "User password was not updated.";
                    return response;
                }

                // Commit changes in this transaction.
                _uow.Commit();

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
        public async Task<IUserServiceResponse> DeleteById(IUserServiceRequest request)
        {
            var response = new UserServiceResponse();

            // Verify required parameters.
            if (request.Id is null || request.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Id (UUID) is required to delete a user by user id.";
                return response;
            }

            try
            {
                // Begin this transaction.
                _uow.Begin();

                // Verify that the user exists.
                var existing = await _uow.users.ReadById((Guid)request.Id);
                if (existing.Id != request.Id)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Requested user id for delete does not exist.";
                    return response;
                }

                // TODO: Figure out what to do with the keepers they are linked to. Just delete those keepers? Seems wrong to do that.
                // TODO: Don't allow a user to delete themselves?

                // Delete any existing user memberships first.
                var membershipsCount = await _uow.memberships.CountByColumnValue("user_id", (Guid)request.Id);
                if (membershipsCount > 0)
                {
                    var successfulDeleteMemberships = await _uow.memberships.DeleteByUserId((Guid)request.Id);

                    // If the user had memberships that couldn't be deleted, rollback and return to the controller.
                    if (!successfulDeleteMemberships)
                    {
                        _uow.Rollback();
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "User was not deleted because existing memberships could not be deleted.";
                        return response;
                    }
                }

                // Delete any existing user permits next.
                var permitsCount = await _uow.permits.CountByColumnValue("user_id", (Guid)request.Id);
                if (permitsCount > 0)
                {
                    var successfulDeletePermits = await _uow.permits.DeleteByUserId((Guid)request.Id);

                    // If the user had permits that couldn't be deleted, rollback and return to the controller.
                    if (!successfulDeletePermits)
                    {
                        _uow.Rollback();
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "User was not deleted because existing permits could not be deleted.";
                        return response;
                    }
                }

                // Delete the user record now.
                var successfulDeleteUser = await _uow.users.DeleteById((Guid)request.Id);

                // If the user record couldn't be deleted, rollback and return to the controller.
                if (!successfulDeleteUser)
                {
                    _uow.Rollback();
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "User was not deleted.";
                    return response;
                }

                // Commit changes in this transaction.
                _uow.Commit();

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
        public async Task<IUserServiceResponse> CreateSession(IUserServiceRequest request)
        {
            var response = new UserServiceResponse();

            // Verify required parameters.
            if (request.UserSessionDto?.Email is null || request.UserSessionDto.Password is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Email (TEXT) and Password (TEXT) are required to authenticate a user.";
                return response;
            }

            try
            {
                // Map the controller's DTO to a domain object for the repository.
                var user = _mapper.Map<User>(request.UserSessionDto);

                var authenticated = await _uow.users.Authenticate(user);

                // Verify that the repository located an actual user record.
                if (authenticated.Id == Guid.Empty)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "No match found for Email and Password.";
                    return response;
                }

                // Map the repository's domain object to a DTO for the response to the controller.
                response.Item = _mapper.Map<UserDto>(authenticated);

                // Create an authentication token for the response to the controller.
                response.Token = _jwt.Encode(authenticated.Id, authenticated.Role);

                // TODO: Update the user's date_last_login.

                // Success if we made it this far.
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
