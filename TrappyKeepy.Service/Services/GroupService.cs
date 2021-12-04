using AutoMapper;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Service
{
    /// <summary>
    /// The group service.
    /// Users may have group memberships. Keeper access (document access) may be
    /// granted to groups (in addition to individual users).
    /// </summary>
    public class GroupService : IGroupService
    {
        /// <summary>
        /// Group database operations into a single transaction (unit of work).
        /// </summary>
        private readonly IUnitOfWork _uow;

        /// <summary>
        /// Automapper http://automapper.org/
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="unitOfWork"></param>
        public GroupService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _uow = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Create a new group in the database.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IGroupServiceResponse> Create(IGroupServiceRequest request)
        {
            var response = new GroupServiceResponse();

            // Verify required parameters.
            if (request.Item?.Name is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Name (TEXT) is required to create a group.";
                return response;
            }

            try
            {
                // Map the controller's DTO to a domain object for the repository.
                var group = _mapper.Map<Group>(request.Item);

                // Begin this transaction.
                _uow.Begin();

                // Verify the requested group name is not already in use.
                var existingNameCount = await _uow.groups.CountByColumnValue("name", group.Name);
                if (existingNameCount > 0)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Requested group name is already in use.";
                    return response;
                }

                // Create the new group record now.
                var newGroup = await _uow.groups.Create(group);

                // Commit changes in this transaction.
                _uow.Commit();

                // Map the repository's domain object to a DTO for the response to the controller.
                response.Item = _mapper.Map<GroupDto>(newGroup);

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
        /// Read all groups from the database.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IGroupServiceResponse> ReadAll(IGroupServiceRequest request)
        {
            var response = new GroupServiceResponse();

            try
            {
                // Read the group records now.
                var groups = await _uow.groups.ReadAll();

                // Map the repository's domain objects to DTOs for the response to the controller.
                var groupDtos = new List<IGroupDto>();
                foreach (var group in groups) groupDtos.Add(_mapper.Map<GroupDto>(group));
                response.List = groupDtos;

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
        /// Read one group from the database.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IGroupServiceResponse> ReadById(IGroupServiceRequest request)
        {
            var response = new GroupServiceResponse();

            // Verify required parameters.
            if (request.Id is null || request.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Id (UUID) is required to find a group by group id.";
                return response;
            }

            try
            {
                // Read the group record now.
                var group = await _uow.groups.ReadById((Guid)request.Id);

                // Verify the user was found.
                if (group.Id != request.Id)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Group was not found with the specified id.";
                    return response;
                }

                // Map the repository's domain object to a DTO for the response to the controller.
                response.Item = _mapper.Map<GroupDto>(group);

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
        /// Update a group in the database.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IGroupServiceResponse> UpdateById(IGroupServiceRequest request)
        {
            var response = new GroupServiceResponse();

            // Verify required parameters.
            if (request.Item?.Id is null || request.Item.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Id (UUID) is required to update a group by group id.";
                return response;
            }

            try
            {
                // Map the controller's DTO to a domain object for the repository.
                var group = _mapper.Map<Group>(request.Item);

                // Begin this transaction.
                _uow.Begin();

                // Verify that the group exists.
                var existing = await _uow.groups.ReadById(group.Id);
                if (existing.Id != request.Item.Id)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Requested group id for update does not exist.";
                    return response;
                }

                // Update the group record now.
                var successful = await _uow.groups.UpdateById(group);

                // If the group record couldn't be updated, rollback and return to the controller.
                if (!successful)
                {
                    _uow.Rollback();
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Group was not updated.";
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
        /// Delete a group from the database.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IGroupServiceResponse> DeleteById(IGroupServiceRequest request)
        {
            var response = new GroupServiceResponse();

            // Verify required parameters.
            if (request.Id is null || request.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Id (UUID) is required to delete a group by group id.";
                return response;
            }

            try
            {
                // Begin this transaction.
                _uow.Begin();

                // Verify that the group exists.
                var existing = await _uow.groups.ReadById((Guid)request.Id);
                if (existing.Id != request.Id)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Requested group id for delete does not exist.";
                    return response;
                }

                // Delete the group record now.
                var successful = await _uow.groups.DeleteById((Guid)request.Id);

                // If the group record couldn't be deleted, rollback and return to the controller.
                if (!successful)
                {
                    _uow.Rollback();
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Group was not deleted.";
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
