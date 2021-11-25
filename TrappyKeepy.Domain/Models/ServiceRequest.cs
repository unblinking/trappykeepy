namespace TrappyKeepy.Domain.Models
{
    /// <summary>
    /// Base service request class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ServiceRequest<T>
    {
        public Guid? Id { get; set; }

        /// <summary>
        /// The item contains the details needed to complete the request.
        /// </summary>
        public T? Item { get; set; }

        // TODO: Also have a token that identifies the requester?
    }

    /// <summary>
    /// User service request.
    /// </summary>
    public class UserServiceRequest : ServiceRequest<UserDto>
    {
        /// <summary>
        ///  If no user is provided, instantiate a new one.
        /// </summary>
        public UserServiceRequest()
        {

        }

        /// <summary>
        /// If a user is provided, set the user as the Item of the request.
        /// Example could be a request to create a new user.
        /// </summary>
        /// <param name="userDto"></param>
        public UserServiceRequest(UserDto userDto)
        {
            this.Item = userDto;
        }

        public UserServiceRequest(Guid id)
        {
            this.Id = id;
        }
    }

    /// <summary>
    /// Keeper service request.
    /// </summary>
    public class KeeperServiceRequest : ServiceRequest<KeeperDto>
    {
        /// <summary>
        ///  If no keeper is provided, instantiate a new one.
        /// </summary>
        public KeeperServiceRequest()
        {

        }

        /// <summary>
        /// If a keeper is provided, set the keeper as the Item of the request.
        /// Example could be a request to create a new keeper.
        /// </summary>
        /// <param name="keeperDto"></param>
        public KeeperServiceRequest(KeeperDto keeperDto)
        {
            this.Item = keeperDto;
        }

        public KeeperServiceRequest(Guid id)
        {
            this.Id = id;
        }
    }
}