using AutoMapper;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Service
{
    /// <summary>
    /// The permit service.
    /// A permit must be issued to a user or a group before they may access a keeper (document).
    /// </summary>
    public class PermitService : IPermitService
    {
        /// <summary>
        /// Group database operations into a single transaction (unit of work).
        /// </summary>
        private readonly IUnitOfWork _uow;

        /// <summary>
        /// Automapper http://automapper.org/
        /// </summary>
        private readonly IMapper _mapper;

        public PermitService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _uow = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Create new permit in the database.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IPermitServiceResponse> Create(IPermitServiceRequest request)
        {
            var response = new PermitServiceResponse();

            // Verify required parameters.
            if (request.Item?.KeeperId is null || request.Item?.KeeperId == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "KeeperId (UUID) is required to create a permit.";
                return response;
            }
            if (
                (request.Item?.UserId is null || request.Item.UserId == Guid.Empty) &&
                (request.Item?.GroupId is null || request.Item.GroupId == Guid.Empty)
            )
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "UserId (UUID) or GroupId (UUID) is required to create a permit.";
                return response;
            }
            // TODO: I'm allowing new permits to be issued for both a group and user in one permit. Any problem with that?

            try
            {
                // Map the controller's DTO to a domain object for the repository.
                var permit = _mapper.Map<Permit>(request.Item);

                // Begin this transaction.
                _uow.Begin();

                // Verify that the KeeperId actually exists.
                var existingKeeper = await _uow.keepers.ReadById(permit.KeeperId);
                if (existingKeeper.Id != permit.KeeperId)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Requested new permit's KeeperId does not exist.";
                    return response;
                }

                // If a UserId was given, verify that the UserId actually exists.
                if (permit.UserId is not null && permit.UserId != Guid.Empty)
                {
                    var existingUser = await _uow.users.ReadById((Guid)permit.UserId);
                    if (existingUser.Id != permit.UserId)
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Requested new permit's UserId does not exist.";
                        return response;
                    }
                }

                // If a GroupId was given, verify that the GroupId actually exists.
                if (permit.GroupId is not null && permit.GroupId != Guid.Empty)
                {
                    var existingGroup = await _uow.groups.ReadById((Guid)permit.GroupId);
                    if (existingGroup.Id != permit.GroupId)
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Requested new permit's GroupId does not exist.";
                        return response;
                    }
                }

                // Verify the requested new permit doesn't already exist.
                var existingCount = await _uow.permits.PermitMatchCount(permit);
                if (existingCount > 0)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Requested new permit already exists.";
                    return response;
                }

                // Create the new permit record now.
                var newPermit = await _uow.permits.Create(permit);

                // Commit changes in this transaction.
                _uow.Commit();

                // Map the repository's domain object to a DTO for the response to the controller.
                response.Item = _mapper.Map<PermitDto>(newPermit);

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
        /// Read all permits from the database by keeper id.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IPermitServiceResponse> ReadByKeeperId(IPermitServiceRequest request)
        {
            var response = new PermitServiceResponse();

            // Verify required parameters.
            if (request.Id is null || request.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Id (UUID) is required to find a permit by keeper id.";
                return response;
            }

            try
            {
                // Read the permit record now.
                var permits = await _uow.permits.ReadByKeeperId((Guid)request.Id);

                // Map the repository's domain objects to DTOs for the response to the controller.
                var permitDtos = new List<IPermitDto>();
                foreach (var permit in permits) permitDtos.Add(_mapper.Map<PermitDto>(permit));
                response.List = permitDtos;

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
        /// Delete a permit from the database.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IPermitServiceResponse> DeleteById(IPermitServiceRequest request)
        {
            var response = new PermitServiceResponse();

            // Verify required parameters.
            if (request.Id is null || request.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Id (UUID) is required to delete a specific permit by id.";
                return response;
            }

            try
            {
                // Begin this transaction.
                _uow.Begin();

                // Verify that the permit exists.
                var existing = await _uow.permits.ReadById((Guid)request.Id);
                if (existing.Id != request.Id)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Requested permit id for delete does not exist.";
                    return response;
                }

                // Delete the permit record now.
                var successful = await _uow.permits.DeleteById((Guid)request.Id);

                // If the permit record could't be deleted, rollback and return to the controller.
                if (!successful)
                {
                    _uow.Rollback();
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Permit was not deleted.";
                    return response;
                }

                // If we made it this far, the permit was deleted. Commit.
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
        /// Delete all permits in the database for a specified keeper id.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IPermitServiceResponse> DeleteByKeeperId(IPermitServiceRequest request)
        {
            var response = new PermitServiceResponse();

            // Verify required parameters.
            if (request.Id is null || request.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Id (UUID) is required to delete permit(s) by keeper id.";
                return response;
            }

            try
            {
                // Begin this transaction.
                _uow.Begin();

                // Verify that at least one permit exists for the keeper.
                var existingCount = await _uow.permits.CountByColumnValue("keeper_id", (Guid)request.Id);
                if (existingCount < 1)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Requested permits(s) for delete not found.";
                    return response;
                }

                // Delete the permit record(s) now.
                var successful = await _uow.permits.DeleteByKeeperId((Guid)request.Id);

                // If the permit record(s) couldn't be deleted, rollback and return to the controller.
                if (!successful)
                {
                    _uow.Rollback();
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Permit(s) not deleted.";
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
        /// Delete all permits in the database for a specified user id.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IPermitServiceResponse> DeleteByUserId(IPermitServiceRequest request)
        {
            var response = new PermitServiceResponse();

            // Verify required parameters.
            if (request.Id is null || request.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Id (UUID) is required to delete permit(s) by user id.";
                return response;
            }

            try
            {
                // Begin this transaction.
                _uow.Begin();

                // Verify that at least one permit exists for the user.
                var existingCount = await _uow.permits.CountByColumnValue("user_id", (Guid)request.Id);
                if (existingCount < 1)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Requested permits(s) for delete not found.";
                    return response;
                }

                // Delete the permit record(s) now.
                var successful = await _uow.permits.DeleteByUserId((Guid)request.Id);

                // If the permit record(s) couldn't be deleted, rollback and return to the controller.
                if (!successful)
                {
                    _uow.Rollback();
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Permit(s) not deleted.";
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
        /// Delete all permits in the database for a specified group id.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IPermitServiceResponse> DeleteByGroupId(IPermitServiceRequest request)
        {
            var response = new PermitServiceResponse();

            // Verify required parameters.
            if (request.Id is null || request.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Id (UUID) is required to delete permit(s) by group id.";
                return response;
            }

            try
            {
                // Begin this transaction.
                _uow.Begin();

                // Verify that at least one permit exists for the group.
                var existingCount = await _uow.permits.CountByColumnValue("group_id", (Guid)request.Id);
                if (existingCount < 1)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Requested permits(s) for delete not found.";
                    return response;
                }

                // Delete the permit record(s) now.
                var successful = await _uow.permits.DeleteByGroupId((Guid)request.Id);

                // If the permit record(s) couldn't be deleted, rollback and return to the controller.
                if (!successful)
                {
                    _uow.Rollback();
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Permit(s) not deleted.";
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
    }
}
