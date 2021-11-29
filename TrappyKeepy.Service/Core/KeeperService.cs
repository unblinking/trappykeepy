using TrappyKeepy.Data;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Models;


namespace TrappyKeepy.Service
{
    public class KeeperService : IKeeperService
    {
        // TODO: Figure out some way to inject UnitOfWork here for testing.
        // TODO: This connection string env var is ugly too.

        private string connectionString;

        public KeeperService()
        {
            this.connectionString = $"{Environment.GetEnvironmentVariable("TKDB_CONN_STRING")}";
        }

        public KeeperService(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<KeeperServiceResponse> Create(KeeperServiceRequest request)
        {
            var response = new KeeperServiceResponse();
            if (request.Item?.Filename is null || request?.BinaryData is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "File name and binary data are required to create a keeper.";
                return response;
            }
            if (request.Item.UserPosted == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "An authorized user is required to create a keeper.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, true))
            {
                try
                {
                    // Verify the requested file name is not already in use.
                    var existingNameCount = await unitOfWork.KeeperRepository
                        .CountByColumnValue("filename", request.Item.Filename);
                    if (existingNameCount > 0)
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "File name already in use.";
                        return response;
                    }

                    // Create the new keeper record now.
                    var id = await unitOfWork.KeeperRepository.Create(request.Item);

                    // Create the new filedata record now.
                    var filedata = new Filedata()
                    {
                        KeeperId = id,
                        BinaryData = request.BinaryData
                    };
                    await unitOfWork.FiledataRepository.Create(filedata);

                    // Commit changes in this transaction.
                    unitOfWork.Commit();

                    // Pass a KeeperDto back to the controller.
                    response.Item = new KeeperDto() { Id = id };

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

        public async Task<KeeperServiceResponse> ReadAll(KeeperServiceRequest request)
        {
            var response = new KeeperServiceResponse();
            using (var unitOfWork = new UnitOfWork(connectionString, false))
            {
                try
                {
                    // Read the keeper records now.
                    var keepers = await unitOfWork.KeeperRepository.ReadAll();

                    // Pass a list of keeperDtos back to the controller.
                    var keeperDtos = new List<KeeperDto>();
                    foreach (var keeper in keepers)
                    {
                        var keeperDto = new KeeperDto()
                        {
                            Id = keeper.Id,
                            Filename = keeper.Filename,
                            Description = keeper.Description,
                            Category = keeper.Category,
                            DatePosted = keeper.DatePosted,
                            UserPosted = keeper.UserPosted,
                        };
                        keeperDtos.Add(keeperDto);
                    }
                    response.List = keeperDtos;

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

        /// <summary>
        /// Read one keeper from the database (meta data), including the filedata (binary data).
        /// </summary>
        /// <param name="request"></param> - A KeeperServiceRequest including the requested keeper id.
        /// <returns>KeeperServiceResponse</returns> - A KeeperDto that includes the BinaryData for the file.
        public async Task<KeeperServiceResponse> ReadById(KeeperServiceRequest request)
        {
            var response = new KeeperServiceResponse();
            if (request.Id is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Requested keeper id was not defined.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, false))
            {
                try
                {
                    // Read the keeper record now.
                    var keeper = await unitOfWork.KeeperRepository.ReadById((Guid)request.Id);

                    // Read the filedata record now.
                    var filedata = await unitOfWork.FiledataRepository.ReadByKeeperId((Guid)request.Id);

                    // Pass a KeeperDto back to the controller.
                    response.Item = new KeeperDto()
                    {
                        Id = keeper.Id,
                        Filename = keeper.Filename,
                        Description = keeper.Description,
                        Category = keeper.Category,
                        DatePosted = keeper.DatePosted,
                        UserPosted = keeper.UserPosted,
                        BinaryData = filedata.BinaryData
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

        public async Task<KeeperServiceResponse> UpdateById(KeeperServiceRequest request)
        {
            var response = new KeeperServiceResponse();
            if (request.Item is null || request.Item.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Keeper id is required to update a keeper.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, true))
            {
                try
                {
                    // Verify that the keeper exists.
                    var existing = await unitOfWork.KeeperRepository.ReadById(request.Item.Id);
                    if (existing.Id != request.Item.Id)
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Requested keeper id for update does not exist.";
                        return response;
                    }

                    // Update the keeper record now.
                    var successful = await unitOfWork.KeeperRepository.UpdateById(request.Item);

                    // If the keeper record couldn't be updated, rollback and return to the controller.
                    if (!successful)
                    {
                        unitOfWork.Rollback();
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Keeper was not updated.";
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

        public async Task<KeeperServiceResponse> DeleteById(KeeperServiceRequest request)
        {
            var response = new KeeperServiceResponse();
            if (request.Id is null || request.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Keeper id is required to delete a keeper.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, true))
            {
                try
                {
                    // Verify that the keeper exists.
                    var existing = await unitOfWork.KeeperRepository.ReadById((Guid)request.Id);
                    if (existing.Id != request.Id)
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Requested keeper id for update does not exist.";
                        return response;
                    }

                    // Delete the keeper record now.
                    var successfulKeeperDelete = await unitOfWork.KeeperRepository.DeleteById((Guid)request.Id);

                    // If the keeper record could't be deleted, rollback and return to the controller.
                    if (!successfulKeeperDelete)
                    {
                        unitOfWork.Rollback();
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Keeper was not deleted.";
                        return response;
                    }

                    // If we made it this far the keeper was deleted. Now delete the associated filedata.
                    var successfulFiledataDelete = await unitOfWork.FiledataRepository.DeleteByKeeperId((Guid)request.Id);

                    // If the filedata record couldn't be deleted, rollback and return to the controller.
                    if (!successfulFiledataDelete)
                    {
                        unitOfWork.Rollback();
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Keeper was not deleted.";
                        return response;
                    }

                    // If we made it this far, both keeper and filedata records were deleted. Commit.
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
