﻿using AutoMapper;
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
        public async Task<PermitServiceResponse> Create(PermitServiceRequest request)
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

                // Map the repository's domain object to a DTO for the controller.
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
        /// Read all permits from the database.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<PermitServiceResponse> ReadAll(PermitServiceRequest request)
        {
            var response = new PermitServiceResponse();

            try
            {
                // Read the permit records now.
                var permits = await _uow.permits.ReadAll();

                // Pass a list of permitDtos back to the controller.
                var permitDtos = new List<PermitDto>();
                foreach (var permit in permits)
                {
                    var permitDto = new PermitDto()
                    {
                        Id = permit.Id,
                        KeeperId = permit.KeeperId,
                        UserId = permit.UserId,
                        GroupId = permit.GroupId,
                    };
                    permitDtos.Add(permitDto);
                }
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
        /// Read all permits from the database by keeper id.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<PermitServiceResponse> ReadByKeeperId(PermitServiceRequest request)
        {
            var response = new PermitServiceResponse();

            // Verify required parameters.
            if (request.Item?.KeeperId is null || request.Item.KeeperId == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "KeeperId (UUID) is required to find a permit by keeper id.";
                return response;
            }

            try
            {
                // Read the permit record now.
                var permits = await _uow.permits.ReadByKeeperId((Guid)request.Item.KeeperId);

                // Map the repository's domain objects to DTOs for the controller.
                var permitDtos = new List<PermitDto>();
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
        /// Read all permits from the database by user id.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<PermitServiceResponse> ReadByUserId(PermitServiceRequest request)
        {
            var response = new PermitServiceResponse();

            // Verify required parameters.
            if (request.Item?.UserId is null || request.Item.UserId == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "UserId (UUID) is required to find a permit by user id.";
                return response;
            }

            try
            {
                // Read the permit record now.
                var permits = await _uow.permits.ReadByUserId((Guid)request.Item.UserId);

                // Map the repository's domain objects to DTOs for the controller.
                var permitDtos = new List<PermitDto>();
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
        /// Read all permits from the database by group id.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<PermitServiceResponse> ReadByGroupId(PermitServiceRequest request)
        {
            var response = new PermitServiceResponse();

            // Verify required parameters.
            if (request.Item?.GroupId is null || request.Item.GroupId == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "GroupId (UUID) is required to find a permit by group id.";
                return response;
            }

            try
            {
                // Read the permit record now.
                var permits = await _uow.permits.ReadByGroupId((Guid)request.Item.GroupId);

                // Map the repository's domain objects to DTOs for the controller.
                var permitDtos = new List<PermitDto>();
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
        public async Task<PermitServiceResponse> DeleteById(PermitServiceRequest request)
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
        public async Task<PermitServiceResponse> DeleteByKeeperId(PermitServiceRequest request)
        {
            var response = new PermitServiceResponse();

            // Verify required parameters.
            if (request.Id is null || request.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Id is required to delete permit(s) by keeper id.";
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
        public async Task<PermitServiceResponse> DeleteByUserId(PermitServiceRequest request)
        {
            var response = new PermitServiceResponse();

            // Verify required parameters.
            if (request.Id is null || request.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Id is required to delete permit(s) by user id.";
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
        public async Task<PermitServiceResponse> DeleteByGroupId(PermitServiceRequest request)
        {
            var response = new PermitServiceResponse();

            // Verify required parameters.
            if (request.Id is null || request.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Id is required to delete permit(s) by group id.";
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
