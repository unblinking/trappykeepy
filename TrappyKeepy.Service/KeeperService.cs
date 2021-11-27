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
            if (request.Item is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Requested new keeper was not defined.";
                return response;
            }
            if (request.Item.Filename is null || request.Item.Binarydata is null || request.BearerToken is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Filename, binary data, and a valid bearer token are required to create a keeper.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, true))
            {
                try
                {
                    // Get the requesting user GUID from the request token.
                    var jwtService = new JwtService();
                    var decodedToken = jwtService.DecodeJwt(request.BearerToken);
                    decodedToken.TryGetValue("id", out var requestingUserGuidObject);
                    if (requestingUserGuidObject is null)
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Request token missing user id.";
                        return response;
                    }
                    var requestingUserGuid = requestingUserGuidObject.ToString();
                    if (requestingUserGuid is null)
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Request token missing user id.";
                        return response;
                    }

                    // TODO: Verify requesting user has permission to make this request.

                    // Received a KeeperDto from the controller. Turn that into a Keeper.

                    var newKeeper = new Keeper()
                    {
                        Filename = request.Item.Filename,
                        DatePosted = DateTime.Now,
                        UserPosted = new Guid(requestingUserGuid)
                    };
                    var existingNameCount = await unitOfWork.KeeperRepository
                        .CountByColumnValue("filename", newKeeper.Filename);
                    if (existingNameCount > 0)
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Requested new keeper filename already in use.";
                        return response;
                    }
                    // Create the new keeper record now.
                    var newKeeperId = await unitOfWork.KeeperRepository.Create(newKeeper);
                    // Create the filedata record now, with the id of the keeper we just created in the database.
                    var newFiledata = new Filedata()
                    {
                        KeeperId = newKeeperId,
                        BinaryData = request.Item.Binarydata
                    };
                    var newFiledataId = await unitOfWork.FiledataRepository.Create(newFiledata);
                    // Commit changes in this transaction.
                    unitOfWork.Commit();
                    // Pass a KeeperDto back to the controller.
                    response.Item = new KeeperDto()
                    {
                        Id = newKeeperId // Id from the database insert.
                    };
                    response.Outcome = OutcomeType.Success;
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    unitOfWork.Dispose();
                    // TODO: Log exception somewhere?
                    response.Outcome = OutcomeType.Error;
                    return response;
                }
            }
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<KeeperServiceResponse> ReadAll(KeeperServiceRequest request)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Read one Keeper from the database (meta data), INCLUDING the actual Filedata (binary data).
        /// </summary>
        /// <param name="request"></param> - A KeeperServiceRequest including the request token and Keeper.Id value.
        /// <returns>KeeperServiceResponse</returns> - A KeeperDto that includes the Binarydata for the document.
        public async Task<KeeperServiceResponse> ReadById(KeeperServiceRequest request)
        {
            var response = new KeeperServiceResponse();
            if (request.Id is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Requested keeper id was not defined.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, true))
            {
                try
                {
                    // TODO: Verify requesting user has permission to make this request.

                    var keeper = await unitOfWork.KeeperRepository.ReadById((Guid)request.Id);
                    var filedata = await unitOfWork.FiledataRepository.ReadByKeeperId((Guid)request.Id);
                    unitOfWork.Commit();

                    // Pass a KeeperDto back to the controller.
                    var keeperDto = new KeeperDto()
                    {
                        Id = keeper.Id,
                        Filename = keeper.Filename,
                        Description = keeper.Description,
                        Category = keeper.Category,
                        DatePosted = keeper.DatePosted,
                        UserPosted = keeper.UserPosted,
                        Binarydata = filedata.BinaryData
                    };
                    response.Item = keeperDto;
                    response.Outcome = OutcomeType.Success;
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    unitOfWork.Dispose();
                    // TODO: Log exception somewhere?
                    response.Outcome = OutcomeType.Error;
                    return response;
                }
            }
            return response;
        }

        public async Task<KeeperServiceResponse> UpdateById(KeeperServiceRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<KeeperServiceResponse> DeleteById(KeeperServiceRequest request)
        {
            throw new NotImplementedException();
        }
    }
}