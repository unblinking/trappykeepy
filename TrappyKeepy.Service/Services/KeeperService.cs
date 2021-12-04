using AutoMapper;
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
        private readonly IUnitOfWork _uow;

        /// <summary>
        /// Automapper http://automapper.org/
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="unitOfWork"></param>
        public KeeperService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _uow = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Create a new keeper (a document) in the database.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IKeeperServiceResponse> Create(IKeeperServiceRequest request)
        {
            var response = new KeeperServiceResponse();

            // Verify required parameters.
            if (request.Item?.UserPosted == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "An authorization bearer token issued to an authorized user is required to create a keeper.";
                return response;
            }
            if (request.Item?.Filename is null || request.Item.ContentType is null || request.BinaryData is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Filename (TEXT), ContentType (TEXT), and File (binary file data sent with the Http request) are required to create a keeper.";
                return response;
            }

            try
            {
                // Map the controller's DTO to a domain object for the repository.
                var keeper = _mapper.Map<Keeper>(request.Item);

                // Begin this transaction.
                _uow.Begin();

                // Verify the user_posted id is a user in the database.
                var user = await _uow.users.ReadById(keeper.UserPosted);
                if (user.Id != keeper.UserPosted)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Bearer token issued to a user Id that does not exist.";
                    return response;
                }

                // TODO: Verify the user has permission to upload (manager or admin).

                // Verify the requested file name is not already in use.
                var existingNameCount = await _uow.keepers.CountByColumnValue("filename", keeper.Filename);
                if (existingNameCount > 0)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "File name already in use.";
                    return response;
                }

                // Create the new keeper record now.
                var newKeeper = await _uow.keepers.Create(keeper);

                // Create the new filedata record now.
                var filedata = new Filedata()
                {
                    KeeperId = newKeeper.Id,
                    BinaryData = request.BinaryData
                };
                await _uow.filedatas.Create(filedata);

                // Commit changes in this transaction.
                _uow.Commit();

                // Map the repository's domain object to a DTO for the response to the controller.
                response.Item = _mapper.Map<KeeperDto>(newKeeper);

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
        public async Task<IKeeperServiceResponse> ReadAll(IKeeperServiceRequest request)
        {
            var response = new KeeperServiceResponse();

            try
            {
                // TODO: Only read the documents that the authorized user has a permit to read.

                // Read the keeper records now.
                var keepers = await _uow.keepers.ReadAll();

                // Map the repository's domain objects to DTOs for the response to the controller.
                var keeperDtos = new List<IKeeperDto>();
                foreach (var keeper in keepers) keeperDtos.Add(_mapper.Map<KeeperDto>(keeper));
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
        public async Task<IKeeperServiceResponse> ReadById(IKeeperServiceRequest request)
        {
            var response = new KeeperServiceResponse();

            // Verify required parameters.
            if (request.Id is null || request.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Id (UUID) is required to find a keeper by keeper id.";
                return response;
            }

            try
            {
                // Read the keeper record now.
                var keeper = await _uow.keepers.ReadById((Guid)request.Id);

                // Read the filedata record now.
                var filedata = await _uow.filedatas.ReadByKeeperId((Guid)request.Id);

                // Map the repository's domain object to a DTO for the response to the controller.
                response.Item = _mapper.Map<KeeperDto>(keeper);
                response.BinaryData = filedata.BinaryData;

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
        public async Task<IKeeperServiceResponse> UpdateById(IKeeperServiceRequest request)
        {
            var response = new KeeperServiceResponse();

            // Verify required parameters.
            if (request.Item?.Id is null || request.Item.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Id (UUID) is required to update a keeper by id.";
                return response;
            }

            try
            {
                // Map the controller's DTO to a domain object for the repository.
                var keeper = _mapper.Map<Keeper>(request.Item);

                // Begin this transaction.
                _uow.Begin();

                // Verify that the keeper exists.
                var existing = await _uow.keepers.ReadById(keeper.Id);
                if (existing.Id != keeper.Id)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Requested keeper id for update does not exist.";
                    return response;
                }

                // Update the keeper record now.
                var successful = await _uow.keepers.UpdateById(keeper);

                // If the keeper record couldn't be updated, rollback and return to the controller.
                if (!successful)
                {
                    _uow.Rollback();
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Keeper was not updated.";
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
        /// Delete a keeper from the database.
        /// This will delete both the tk.keepers metadata record, and the
        /// tk.filedata binary data record.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IKeeperServiceResponse> DeleteById(IKeeperServiceRequest request)
        {
            var response = new KeeperServiceResponse();

            // Verify required parameters.
            if (request.Id is null || request.Id == Guid.Empty)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Id (UUID) is required to delete a keeper by keeper id.";
                return response;
            }

            try
            {
                // Begin this transaction.
                _uow.Begin();

                // Verify that the keeper exists.
                var existing = await _uow.keepers.ReadById((Guid)request.Id);
                if (existing.Id != request.Id)
                {
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Requested keeper id for delete does not exist.";
                    return response;
                }

                // Delete the keeper record now.
                var successfulKeeperDelete = await _uow.keepers.DeleteById((Guid)request.Id);

                // If the keeper record could't be deleted, rollback and return to the controller.
                if (!successfulKeeperDelete)
                {
                    _uow.Rollback();
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Keeper was not deleted.";
                    return response;
                }

                // If we made it this far the keeper was deleted. Now delete the associated filedata.
                var successfulFiledataDelete = await _uow.filedatas.DeleteByKeeperId((Guid)request.Id);

                // If the filedata record couldn't be deleted, rollback and return to the controller.
                if (!successfulFiledataDelete)
                {
                    _uow.Rollback();
                    response.Outcome = OutcomeType.Fail;
                    response.ErrorMessage = "Keeper was not deleted.";
                    return response;
                }

                // If we made it this far, both keeper and filedata records were deleted. Commit.
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
