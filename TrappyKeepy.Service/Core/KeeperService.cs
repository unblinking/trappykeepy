using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Service
{
    /// <summary>
    /// The keeper service.
    /// A keeper is a document worth keeping. Keepers are stored in two database
    /// tables. The tk.keepers table stores document metadata. The tk.filedatas
    /// table stores document binary file data.
    /// </summary>
    public class KeeperService : IKeeperService
    {
        /// <summary>
        /// Group database operations into a single transaction (unit of work).
        /// </summary>
        private readonly IUnitOfWork uow;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="unitOfWork"></param>
        public KeeperService(IUnitOfWork unitOfWork)
        {
            this.uow = unitOfWork;
        }

        /// <summary>
        /// Create a new keeper (a document) in the database.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<KeeperServiceResponse> Create(KeeperServiceRequest request)
        {
            var response = new KeeperServiceResponse();

            // Verify required parameters.
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

            try
            {
                // Begin this transaction.
                uow.Begin();

                // Verify the requested file name is not already in use.
                var existingNameCount = await uow.keepers.CountByColumnValue("filename", request.Item.Filename);
                if (existingNameCount > 0)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "File name already in use.";
                    return response;
                }

                // Create the new keeper record now.
                var id = await uow.keepers.Create(request.Item);

                // Create the new filedata record now.
                var filedata = new Filedata()
                {
                    KeeperId = id,
                    BinaryData = request.BinaryData
                };
                await uow.filedatas.Create(filedata);

                // Commit changes in this transaction.
                uow.Commit();

                // Pass a KeeperDto back to the controller.
                response.Item = new KeeperDto() { Id = id };

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
        /// Read all keepers (all documents) from the database.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<KeeperServiceResponse> ReadAll(KeeperServiceRequest request)
        {
            var response = new KeeperServiceResponse();

            try
            {
                // Read the keeper records now.
                var keepers = await uow.keepers.ReadAll();

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
                response.Outcome = OutcomeType.Error;
            }

            return response;
        }

        /// <summary>
        /// Read one keeper from the database (metadata), including the filedata (binary data).
        /// </summary>
        /// <param name="request">A KeeperServiceRequest including the requested keeper id.</param>
        /// <returns>KeeperServiceResponse - A KeeperDto that includes the BinaryData for the file.</returns>
        public async Task<KeeperServiceResponse> ReadById(KeeperServiceRequest request)
        {
            var response = new KeeperServiceResponse();

            // Verify required parameters.
            if (request.Id is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Keeper id is required to find a keeper by id.";
                return response;
            }

            try
            {
                // Read the keeper record now.
                var keeper = await uow.keepers.ReadById((Guid)request.Id);

                // Read the filedata record now.
                var filedata = await uow.filedatas.ReadByKeeperId((Guid)request.Id);

                // Pass a KeeperDto back to the controller. This DTO includes the filedata (binary data).
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
                response.Outcome = OutcomeType.Error;
            }

            return response;
        }

        /// <summary>
        /// Update a keeper (a document) in the database.
        /// Only metadata (the tk.keepers table) may be updated. The binary file
        /// data (tk.filedatas table) may not be updated. To "update" the binary
        /// file data, delete the old keeper and create a new keeper.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<KeeperServiceResponse> UpdateById(KeeperServiceRequest request)
        {
            var response = new KeeperServiceResponse();

            // Verify required parameters.
            if (request.Item is null || request.Item.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Keeper id is required to update a keeper by id.";
                return response;
            }

            try
            {
                // Begin this transaction.
                uow.Begin();

                // Verify that the keeper exists.
                var existing = await uow.keepers.ReadById(request.Item.Id);
                if (existing.Id != request.Item.Id)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Requested keeper id for update does not exist.";
                    return response;
                }

                // Update the keeper record now.
                var successful = await uow.keepers.UpdateById(request.Item);

                // If the keeper record couldn't be updated, rollback and return to the controller.
                if (!successful)
                {
                    uow.Rollback();
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Keeper was not updated.";
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
        /// Delete a keeper from the database.
        /// This will delete both the tk.keepers metadata record, and the
        /// tk.filedata binary data record.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<KeeperServiceResponse> DeleteById(KeeperServiceRequest request)
        {
            var response = new KeeperServiceResponse();

            // Verify required parameters.
            if (request.Id is null || request.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Keeper id is required to delete a keeper by id.";
                return response;
            }

            try
            {
                // Begin this transaction.
                uow.Begin();

                // Verify that the keeper exists.
                var existing = await uow.keepers.ReadById((Guid)request.Id);
                if (existing.Id != request.Id)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Requested keeper id for update does not exist.";
                    return response;
                }

                // Delete the keeper record now.
                var successfulKeeperDelete = await uow.keepers.DeleteById((Guid)request.Id);

                // If the keeper record could't be deleted, rollback and return to the controller.
                if (!successfulKeeperDelete)
                {
                    uow.Rollback();
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Keeper was not deleted.";
                    return response;
                }

                // If we made it this far the keeper was deleted. Now delete the associated filedata.
                var successfulFiledataDelete = await uow.filedatas.DeleteByKeeperId((Guid)request.Id);

                // If the filedata record couldn't be deleted, rollback and return to the controller.
                if (!successfulFiledataDelete)
                {
                    uow.Rollback();
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Keeper was not deleted.";
                    return response;
                }

                // If we made it this far, both keeper and filedata records were deleted. Commit.
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
