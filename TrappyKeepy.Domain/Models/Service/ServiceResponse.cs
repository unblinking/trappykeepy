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

    /// <summary>
    /// Group service response class.
    /// </summary>
    public class GroupServiceResponse : ServiceResponse<GroupDto>
    {
        /// <summary>
        /// If no group is provided, instantiate a new one in the Item.
        /// </summary>
        public GroupServiceResponse()
        {
            this.Item = new GroupDto();
        }

        /// <summary>
        /// If a group was provided, set the group to the response Item.
        /// Example could be a lookup for a single group by Id.
        /// </summary>
        /// <param name="groupDto"></param>
        public GroupServiceResponse(GroupDto groupDto)
        {
            this.Item = groupDto;
        }

        /// <summary>
        /// If a list of groups was provided, set the list to the response Item.
        /// Example could be a lookup for all groups.
        /// </summary>
        /// <param name="groupDtos"></param>
        public GroupServiceResponse(List<GroupDto> groupDtos)
        {
            this.List = groupDtos;
        }
    }

    /// <summary>
    /// Membership service response class.
    /// </summary>
    public class MembershipServiceResponse : ServiceResponse<MembershipDto>
    {
        /// <summary>
        /// If no membership is provided, instantiate a new one in the Item.
        /// </summary>
        public MembershipServiceResponse()
        {
            this.Item = new MembershipDto();
        }

        /// <summary>
        /// If a membership was provided, set the membership to the response Item.
        /// Example could be a lookup for a single membership by Id.
        /// </summary>
        /// <param name="membershipDto"></param>
        public MembershipServiceResponse(MembershipDto membershipDto)
        {
            this.Item = membershipDto;
        }

        /// <summary>
        /// If a list of memberships was provided, set the list to the response Item.
        /// Example could be a lookup for all memberships.
        /// </summary>
        /// <param name="membershipDtos"></param>
        public MembershipServiceResponse(List<MembershipDto> membershipDtos)
        {
            this.List = membershipDtos;
        }
    }

    /// <summary>
    /// Permit service response class.
    /// </summary>
    public class PermitServiceResponse : ServiceResponse<PermitDto>
    {
        /// <summary>
        /// If no permit is provided, instantiate a new one in the Item.
        /// </summary>
        public PermitServiceResponse()
        {
            this.Item = new PermitDto();
        }

        /// <summary>
        /// If a permit was provided, set the permit to the response Item.
        /// Example could be a lookup for a single permit by keeper id.
        /// </summary>
        /// <param name="permitDto"></param>
        public PermitServiceResponse(PermitDto permitDto)
        {
            this.Item = permitDto;
        }

        /// <summary>
        /// If a list of permits was provided, set the list to the response Item.
        /// Example could be a lookup for all permits by keeper id.
        /// </summary>
        /// <param name="permitsDtos"></param>
        public PermitServiceResponse(List<PermitDto> permitDtos)
        {
            this.List = permitDtos;
        }
    }
}
