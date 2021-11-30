using TrappyKeepy.Data;
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
        private readonly IUnitOfWork uow;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="unitOfWork"></param>
        public GroupService(IUnitOfWork unitOfWork)
        {
            this.uow = unitOfWork;
        }

        /// <summary>
        /// Create a new group in the database.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GroupServiceResponse> Create(GroupServiceRequest request)
        {
            var response = new GroupServiceResponse();

            // Verify required parameters.
            if (request.Item?.Name is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Name is required to create a group.";
                return response;
            }

            try
            {
                // Begin this transaction.
                uow.Begin();

                // Verify the requested group name is not already in use.
                var existingNameCount = await uow.groups.CountByColumnValue("name", request.Item.Name);
                if (existingNameCount > 0)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Requested group name is already in use.";
                    return response;
                }

                // Create the new group record now.
                var id = await uow.groups.Create(request.Item);

                // Commit changes in this transaction.
                uow.Commit();

                // Pass a GroupDto back to the controller.
                response.Item = new GroupDto() { Id = id };

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
        public async Task<GroupServiceResponse> ReadAll(GroupServiceRequest request)
        {
            var response = new GroupServiceResponse();

            try
            {
                // Read the group records now.
                var groups = await uow.groups.ReadAll();

                // Pass a list of groupDtos back to the controller.
                var groupDtos = new List<GroupDto>();
                foreach (var group in groups)
                {
                    var groupDto = new GroupDto()
                    {
                        Id = group.Id,
                        Name = group.Name,
                        Description = group.Description,
                        DateCreated = group.DateCreated,
                    };
                    groupDtos.Add(groupDto);
                }
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
        public async Task<GroupServiceResponse> ReadById(GroupServiceRequest request)
        {
            var response = new GroupServiceResponse();

            // Verify required parameters.
            if (request.Id is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Group id is required to find a group by id.";
                return response;
            }

            try
            {
                // Read the group record now.
                var group = await uow.groups.ReadById((Guid)request.Id);

                // Pass a GroupDto back to the controller.
                response.Item = new GroupDto()
                {
                    Id = group.Id,
                    Name = group.Name,
                    Description = group.Description,
                    DateCreated = group.DateCreated,
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
        /// Update a group in the database.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GroupServiceResponse> UpdateById(GroupServiceRequest request)
        {
            var response = new GroupServiceResponse();

            // Verify required parameters.
            if (request.Item is null || request.Item.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Group id is required to update a group by id.";
                return response;
            }

            try
            {
                // Begin this transaction.
                uow.Begin();

                // Verify that the group exists.
                var existing = await uow.groups.ReadById(request.Item.Id);
                if (existing.Id != request.Item.Id)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Requested group id for update does not exist.";
                    return response;
                }

                // Update the group record now.
                var successful = await uow.groups.UpdateById(request.Item);

                // If the group record couldn't be updated, rollback and return to the controller.
                if (!successful)
                {
                    uow.Rollback();
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Group was not updated.";
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
        /// Delete a group from the database.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GroupServiceResponse> DeleteById(GroupServiceRequest request)
        {
            var response = new GroupServiceResponse();

            // Verify required parameters.
            if (request.Id is null || request.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Group id is required to delete a group by id.";
                return response;
            }

            try
            {
                // Begin this transaction.
                uow.Begin();

                // Verify that the group exists.
                var existing = await uow.groups.ReadById((Guid)request.Id);
                if (existing.Id != request.Id)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Requested group id for delete does not exist.";
                    return response;
                }

                // Delete the group record now.
                var successful = await uow.groups.DeleteById((Guid)request.Id);

                // If the group record couldn't be deleted, rollback and return to the controller.
                if (!successful)
                {
                    uow.Rollback();
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Group was not deleted.";
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
    }
}
