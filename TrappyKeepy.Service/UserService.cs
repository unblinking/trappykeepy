using TrappyKeepy.Data;
using TrappyKeepy.Domain;
using TrappyKeepy.Domain.Interfaces;

namespace TrappyKeepy.Service
{
    public class UserService : IUserService
    {
        private string connectionString;

        public UserService() {
            var envVar = $"{Environment.GetEnvironmentVariable("TKDB_CONN_STRING")}";
            this.connectionString = envVar;
        }

        /*
        public async Task<User> Create(User user)
        {

            return newUser;
        }
        */

        public async Task<List<User>> ReadAll() {
            var users = new List<User>();
            using (var unitOfWork = new UnitOfWork(connectionString, true))
            {
                try {
                    users = await unitOfWork.UserRepository.ReadAll();
                    unitOfWork.Commit();
                } catch (Exception) {
                    unitOfWork.Rollback();
                    throw;
                }
            }
            return users;
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