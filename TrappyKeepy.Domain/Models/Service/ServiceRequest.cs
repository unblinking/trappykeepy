namespace TrappyKeepy.Domain.Models
{
    public interface IServiceRequest<T>
    {
        Guid? Id { get; }
        T? Item { get; }
    }

    /// <summary>
    /// Base service request class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ServiceRequest<T> : IServiceRequest<T>
    {
        public Guid? Id { get; set; }

        /// <summary>
        /// The item contains the details needed to complete the request.
        /// </summary>
        public T? Item { get; set; }
    }

    public interface IUserServiceRequest : IServiceRequest<IUserDto>
    {
        IUserSessionDto? UserSessionDto { get; }
    }

    /// <summary>
    /// User service request.
    /// </summary>
    public class UserServiceRequest : ServiceRequest<IUserDto>, IUserServiceRequest
    {
        public IUserSessionDto? UserSessionDto { get; set; }

        /// <summary>
        ///  If no user is provided, instantiate a new one.
        /// </summary>
        public UserServiceRequest()
        {
            Item = new UserDto();
        }

        /// <summary>
        /// If a user is provided, set the user as the Item of the request.
        /// Example could be a request to create a new user.
        /// </summary>
        /// <param name="user"></param>
        public UserServiceRequest(IUserDto user)
        {
            Item = user;
        }

        public UserServiceRequest(IUserSessionDto user)
        {
            UserSessionDto = user;
        }

        public UserServiceRequest(Guid id)
        {
            Id = id;
        }
    }

    public interface IKeeperServiceRequest : IServiceRequest<IKeeperDto>
    {
        byte[]? BinaryData { get; }
        Guid? RequestingUserId { get; }
    }

    /// <summary>
    /// Keeper service request.
    /// </summary>
    public class KeeperServiceRequest : ServiceRequest<IKeeperDto>, IKeeperServiceRequest
    {
        /// <summary>
        /// The associated Filedata.BinaryData value.
        /// </summary>
        public byte[]? BinaryData { get; set; }

        public Guid? RequestingUserId { get; set; }

        /// <summary>
        ///  If no keeper is provided, instantiate a new one.
        /// </summary>
        public KeeperServiceRequest()
        {
            Item = new KeeperDto();
        }

        /// <summary>
        /// If a keeper is provided, set the keeper as the Item of the request.
        /// Example could be a request to create a new keeper.
        /// </summary>
        /// <param name="keeper"></param>
        public KeeperServiceRequest(IKeeperDto keeper)
        {
            Item = keeper;
        }

        public KeeperServiceRequest(Guid id)
        {
            Id = id;
        }
    }

    public interface IGroupServiceRequest : IServiceRequest<IGroupDto>
    {

    }

    /// <summary>
    /// Group service request.
    /// </summary>
    public class GroupServiceRequest : ServiceRequest<IGroupDto>, IGroupServiceRequest
    {
        /// <summary>
        ///  If no group is provided, instantiate a new one.
        /// </summary>
        public GroupServiceRequest()
        {
            Item = new GroupDto();
        }

        /// <summary>
        /// If a group is provided, set the group as the Item of the request.
        /// Example could be a request to create a new group.
        /// </summary>
        /// <param name="group"></param>
        public GroupServiceRequest(IGroupDto group)
        {
            Item = group;
        }

        public GroupServiceRequest(Guid id)
        {
            Id = id;
        }
    }

    public interface IMembershipServiceRequest : IServiceRequest<IMembershipDto>
    {

    }

    /// <summary>
    /// Membership service request.
    /// </summary>
    public class MembershipServiceRequest : ServiceRequest<IMembershipDto>, IMembershipServiceRequest
    {
        /// <summary>
        ///  If no membership is provided, instantiate a new one.
        /// </summary>
        public MembershipServiceRequest()
        {
            Item = new MembershipDto();
        }

        /// <summary>
        /// If a membership is provided, set the membership as the Item of the
        /// request. Example could be a request to create a new membership.
        /// </summary>
        /// <param name="membership"></param>
        public MembershipServiceRequest(IMembershipDto membership)
        {
            Item = membership;
        }

        /// <summary>
        /// Used to request memberships by group id or user id, or delete
        /// memberships by group id or user id.
        /// </summary>
        /// <param name="id"></param>
        public MembershipServiceRequest(Guid id)
        {
            Id = id;
        }
    }

    public interface IPermitServiceRequest : IServiceRequest<IPermitDto>
    {

    }

    /// <summary>
    /// Permit service request.
    /// </summary>
    public class PermitServiceRequest : ServiceRequest<IPermitDto>, IPermitServiceRequest
    {
        /// <summary>
        ///  If no permit is provided, instantiate a new one.
        /// </summary>
        public PermitServiceRequest()
        {
            Item = new PermitDto();
        }

        /// <summary>
        /// If a permit is provided, set the permit as the Item of the
        /// request. Example could be a request to create a new permit.
        /// </summary>
        /// <param name="permit"></param>
        public PermitServiceRequest(IPermitDto permit)
        {
            Item = permit;
        }

        /// <summary>
        /// Used to request permits by group id or user id, or delete
        /// permits by group id or user id.
        /// </summary>
        /// <param name="id"></param>
        public PermitServiceRequest(Guid id)
        {
            Id = id;
        }
    }
}
