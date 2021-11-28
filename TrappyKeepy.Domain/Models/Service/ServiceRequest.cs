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

        public Guid? RequesterId { get; set; }
    }

    /// <summary>
    /// User service request.
    /// </summary>
    public class UserServiceRequest : ServiceRequest<User>
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
        /// <param name="user"></param>
        public UserServiceRequest(User user)
        {
            this.Item = user;
        }

        public UserServiceRequest(Guid id)
        {
            this.Id = id;
        }
    }

    /// <summary>
    /// Keeper service request.
    /// </summary>
    public class KeeperServiceRequest : ServiceRequest<Keeper>
    {
        public byte[]? BinaryData { get; set; }

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
        /// <param name="keeper"></param>
        public KeeperServiceRequest(Keeper keeper)
        {
            this.Item = keeper;
        }

        public KeeperServiceRequest(Guid id)
        {
            this.Id = id;
        }
    }
}
