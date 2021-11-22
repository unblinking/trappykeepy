namespace TrappyKeepy.Domain.Models
{
    /// <summary>
    /// Possible outcomes after the service attempts to process the request.
    /// </summary>
    public enum OutcomeType : int
    {
        Error = 0,
        Fail = 1,
        Success = 2,
    }

    /// <summary>
    /// Base service response class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ServiceResponse<T>
    {
        public OutcomeType Outcome { get; set; } = OutcomeType.Error;
        public string? ErrorMessage { get; set; }
        public Guid? Id { get; set; }
        public T? Item { get; set; }
        public List<T>? List { get; set; }
    }

    /// <summary>
    /// User service response class.
    /// </summary>
    public class UserServiceResponse : ServiceResponse<User>
    {
        /// <summary>
        /// If no user is provided, instantiate a new one in the Item.
        /// </summary>
        public UserServiceResponse()
        {
            this.Item = new User();
        }

        /// <summary>
        /// If a user was provided, set the user to the response Item.
        /// Example could be a lookup for a single user by Id.
        /// </summary>
        /// <param name="user"></param>
        public UserServiceResponse(User user)
        {
            this.Item = user;
        }

        /// <summary>
        /// If a list of users was provided, set the list to the response Item.
        /// Example could be a lookup for all users.
        /// </summary>
        /// <param name="users"></param>
        public UserServiceResponse(List<User> users)
        {
            this.List = users;
        }
    }
}