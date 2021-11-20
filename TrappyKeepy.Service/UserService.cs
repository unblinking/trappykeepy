using TrappyKeepy.Data;
using TrappyKeepy.Domain;
using TrappyKeepy.Domain.Interfaces;

namespace TrappyKeepy.Service
{
    public class UserService : IUserService
    {
        private string connectionString;

        public UserService() {
            var envVar = $"{Environment.GetEnvironmentVariable("TKDB")}";
            this.connectionString = envVar;
        }

        /*
        public async Task<User> Create(User user)
        {

            return newUser;
        }
        */

        public async Task<List<User>> ReadAll() {
            using (var unitOfWork = new UnitOfWork(connectionString, true))
            {
                var users = await unitOfWork.UserRepository.ReadAll();
                unitOfWork.Commit();
                return users;
            }
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
