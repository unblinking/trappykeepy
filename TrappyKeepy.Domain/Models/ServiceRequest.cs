namespace TrappyKeepy.Domain.Models
{
    /// <summary>
    /// Base service request class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ServiceRequest<T>
    {
        /// <summary>
        /// The item contains the details needed to complete the request.
        /// </summary>
        public T? Item { get; set; }
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
            this.Item = new User();
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
    }
}