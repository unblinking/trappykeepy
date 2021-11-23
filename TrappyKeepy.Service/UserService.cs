using TrappyKeepy.Data;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Service
{
    public class UserService : IUserService
    {
        private string connectionString;

        public UserService()
        {
            this.connectionString = $"{Environment.GetEnvironmentVariable("TKDB_CONN_STRING")}";
        }

        public UserService(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<UserServiceResponse> Create(UserServiceRequest request)
        {
            var response = new UserServiceResponse();
            // TODO: Verify requesting user has permission to make this request.
            if (request.Item is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Requested new user was not defined.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, false))
            {
                try
                {
                    var existingNameCount = await unitOfWork.UserRepository
                        .CountByColumnValue("name", request.Item.Name);
                    if (existingNameCount > 0)
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Requested new user name already in use.";
                        return response;
                    }
                    var existingEmailCount = await unitOfWork.UserRepository
                        .CountByColumnValue("email", request.Item.Email);
                    if (existingEmailCount > 0)
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Requested new user email already in use.";
                        return response;
                    }
                    var newId = await unitOfWork.UserRepository.Create(request.Item);
                    unitOfWork.Commit();
                    response.Id = newId;
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

        public async Task<UserServiceResponse> ReadAll(UserServiceRequest request)
        {
            var response = new UserServiceResponse();
            // TODO: Verify requesting user has permission to make this request.
            using (var unitOfWork = new UnitOfWork(connectionString, true))
            {
                try
                {
                    response.List = await unitOfWork.UserRepository.ReadAll();
                    unitOfWork.Commit();
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

        public async Task<UserServiceResponse> ReadById(UserServiceRequest request)
        {
            var response = new UserServiceResponse();
            // TODO: Verify requesting user has permission to make this request.
            if (request.Id is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Requested user Id was not defined.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, true))
            {
                try
                {
                    response.Item = await unitOfWork.UserRepository.ReadById((Guid)request.Id);
                    unitOfWork.Commit();
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

        public async Task<UserServiceResponse> UpdateById(UserServiceRequest request)
        {
            var response = new UserServiceResponse();
            // TODO: Verify requesting user has permission to make this request.
            if (request.Item is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Requested user to be updated was not defined.";
                return response;
            }
            using (var unitOfWork = new UnitOfWork(connectionString, true))
            {
                try
                {
                    // TODO: Verify that the user exists first?
                    var successful = await unitOfWork.UserRepository.UpdateById(request.Item);
                    unitOfWork.Commit();
                    if (successful)
                    {
                        response.Outcome = OutcomeType.Success;
                    }
                    else
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "User was not updated.";
                    }
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

        /*
        public async Task<UserServiceResponse> DeleteById(UserServiceRequest request)
        {

            return response;
        }
        */
    }
}