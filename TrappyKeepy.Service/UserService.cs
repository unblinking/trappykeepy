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
            if (request.Item is null)
            {
                response.Outcome = OutcomeType.Fail;
                response.ErrorMessage = "Requested new user was not defined.";
                return response;
            }
            // TODO: Verify requesting user has permission to make this request.
            using (var unitOfWork = new UnitOfWork(connectionString, false))
            {
                try
                {
                    var existingUserCount = await unitOfWork.UserRepository.CountByUsername(request.Item.Name);
                    if (existingUserCount > 0)
                    {
                        response.Outcome = OutcomeType.Fail;
                        response.ErrorMessage = "Requested new user name already in use.";
                        return response;
                    }
                    // TODO: Verify user with email doesn't already exist.
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

        /*
        public async Task<User> ReadById(Guid id)
        {

            return user;
        }

        public async Task<User> Update(User user)
        {

            return updatedUser;
        }

        public async Task<User> Delete(Guid id)
        {

            return deletedUser;
        }
        */
    }
}