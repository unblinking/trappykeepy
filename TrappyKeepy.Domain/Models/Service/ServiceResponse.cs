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
        public string? Token { get; set; }
    }

    /// <summary>
    /// User service response class.
    /// </summary>
    public class UserServiceResponse : ServiceResponse<UserDto>
    {
        /// <summary>
        /// If no user is provided, instantiate a new one in the Item.
        /// </summary>
        public UserServiceResponse()
        {
            this.Item = new UserDto();
        }

        /// <summary>
        /// If a user was provided, set the user to the response Item.
        /// Example could be a lookup for a single user by Id.
        /// </summary>
        /// <param name="userDto"></param>
        public UserServiceResponse(UserDto userDto)
        {
            this.Item = userDto;
        }

        /// <summary>
        /// If a list of users was provided, set the list to the response Item.
        /// Example could be a lookup for all users.
        /// </summary>
        /// <param name="userDtos"></param>
        public UserServiceResponse(List<UserDto> userDtos)
        {
            this.List = userDtos;
        }
    }

    /// <summary>
    /// Keeper service response class.
    /// </summary>
    public class KeeperServiceResponse : ServiceResponse<KeeperDto>
    {
        /// <summary>
        /// If no keeper is provided, instantiate a new one in the Item.
        /// </summary>
        public KeeperServiceResponse()
        {
            this.Item = new KeeperDto();
        }

        /// <summary>
        /// If a keeper was provided, set the keeper to the response Item.
        /// Example could be a lookup for a single keeper by Id.
        /// </summary>
        /// <param name="keeperDto"></param>
        public KeeperServiceResponse(KeeperDto keeperDto)
        {
            this.Item = keeperDto;
        }

        /// <summary>
        /// If a list of keepers was provided, set the list to the response Item.
        /// Example could be a lookup for all keepers.
        /// </summary>
        /// <param name="keeperDtos"></param>
        public KeeperServiceResponse(List<KeeperDto> keeperDtos)
        {
            this.List = keeperDtos;
        }
    }
}
