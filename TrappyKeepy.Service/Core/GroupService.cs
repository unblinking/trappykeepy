using TrappyKeepy.Data;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Service
{
    public class GroupService : IGroupService
    {
        // TODO: Figure out some way to inject UnitOfWork here for testing.
        // TODO: This connection string env var is ugly too.

        private string connectionString;

        public GroupService()
        {
            this.connectionString = $"{Environment.GetEnvironmentVariable("TKDB_CONN_STRING")}";
        }

        public GroupService(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<GroupServiceResponse> Create(GroupServiceRequest request)
        {
            var response = new GroupServiceResponse();
            if (request.Item?.Name is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Name is required to create a group.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, true))
            {
                try
                {
                    // Verify the requested group name is not already in use.
                    var existingNameCount = await unitOfWork.GroupRepository
                        .CountByColumnValue("name", request.Item.Name);
                    if (existingNameCount > 0)
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Requested group name is already in use.";
                        return response;
                    }

                    // Create the new group record now.
                    var id = await unitOfWork.GroupRepository.Create(request.Item);

                    // Commit changes in this transaction.
                    unitOfWork.Commit();

                    // Pass a GroupDto back to the controller.
                    response.Item = new GroupDto() { Id = id };

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

        public async Task<GroupServiceResponse> ReadAll(GroupServiceRequest request)
        {
            var response = new GroupServiceResponse();
            using (var unitOfWork = new UnitOfWork(connectionString, false))
            {
                try
                {
                    // Read the group records now.
                    var groups = await unitOfWork.GroupRepository.ReadAll();

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
                    unitOfWork.Rollback();
                    unitOfWork.Dispose();
                    response.Outcome = OutcomeType.Error;
                    return response;
                }
            }
            return response;
        }

        public async Task<GroupServiceResponse> ReadById(GroupServiceRequest request)
        {
            var response = new GroupServiceResponse();
            if (request.Id is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Group id is required to find a group.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, false))
            {
                try
                {
                    // Read the group record now.
                    var group = await unitOfWork.GroupRepository.ReadById((Guid)request.Id);

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
                    unitOfWork.Rollback();
                    unitOfWork.Dispose();
                    response.Outcome = OutcomeType.Error;
                    return response;
                }
            }
            return response;
        }

        public async Task<GroupServiceResponse> UpdateById(GroupServiceRequest request)
        {
            var response = new GroupServiceResponse();
            if (request.Item is null || request.Item.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Group id is required to update a group.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, true))
            {
                try
                {
                    // Verify that the group exists.
                    var existing = await unitOfWork.GroupRepository.ReadById(request.Item.Id);
                    if (existing.Id != request.Item.Id)
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Requested group id for update does not exist.";
                        return response;
                    }

                    // Update the group record now.
                    var successful = await unitOfWork.GroupRepository.UpdateById(request.Item);

                    // If the group record couldn't be updated, rollback and return to the controller.
                    if (!successful)
                    {
                        unitOfWork.Rollback();
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Group was not updated.";
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

        public async Task<GroupServiceResponse> DeleteById(GroupServiceRequest request)
        {
            var response = new GroupServiceResponse();
            if (request.Id is null || request.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Group id is required to delete a group.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, true))
            {
                try
                {
                    // Verify that the group exists.
                    var existing = await unitOfWork.GroupRepository.ReadById((Guid)request.Id);
                    if (existing.Id != request.Id)
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Requested group id for delete does not exist.";
                        return response;
                    }

                    // Delete the group record now.
                    var successful = await unitOfWork.GroupRepository.DeleteById((Guid)request.Id);

                    // If the group record couldn't be deleted, rollback and return to the controller.
                    if (!successful)
                    {
                        unitOfWork.Rollback();
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Group was not deleted.";
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
    }
}
