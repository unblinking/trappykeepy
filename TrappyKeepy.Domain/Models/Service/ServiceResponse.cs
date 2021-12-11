using Microsoft.AspNetCore.Mvc;

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

    public interface IServiceResponse<T>
    {
        OutcomeType Outcome { get; }
        string? ErrorMessage { get; }
        Guid? Id { get; }
        T? Item { get; }
        List<T>? List { get; }
        string? Token { get; }
    }

    /// <summary>
    /// Base service response class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ServiceResponse<T> : IServiceResponse<T>
    {
        public OutcomeType Outcome { get; set; } = OutcomeType.Error;
        public string? ErrorMessage { get; set; }
        public Guid? Id { get; set; }
        public T? Item { get; set; }
        public List<T>? List { get; set; }
        public string? Token { get; set; }
    }

    public interface IUserServiceResponse : IServiceResponse<IUserDto>
    {
        IUserComplexDto? ComplexDto { get; }
    }

    /// <summary>
    /// User service response class.
    /// </summary>
    public class UserServiceResponse : ServiceResponse<IUserDto>, IUserServiceResponse
    {
        public IUserComplexDto? ComplexDto { get; set; }

        /// <summary>
        /// If no user is provided, leave things null.
        /// </summary>
        public UserServiceResponse()
        {

        }

        /// <summary>
        /// If a user was provided, set the user to the response Item.
        /// Example could be a lookup for a single user by Id.
        /// </summary>
        /// <param name="user"></param>
        public UserServiceResponse(IUserDto user)
        {
            Item = user;
        }

        /// <summary>
        /// If a list of users was provided, set the list to the response Item.
        /// Example could be a lookup for all users.
        /// </summary>
        /// <param name="user"></param>
        public UserServiceResponse(List<IUserDto> user)
        {
            List = user;
        }
    }

    public interface IKeeperServiceResponse : IServiceResponse<IKeeperDto>
    {
        byte[]? BinaryData { get; }

        FileContentResult? FileContentResult { get; }
    }

    /// <summary>
    /// Keeper service response class.
    /// </summary>
    public class KeeperServiceResponse : ServiceResponse<IKeeperDto>, IKeeperServiceResponse
    {
        /// <summary>
        /// The associated Filedata.BinaryData value.
        /// </summary>
        public byte[]? BinaryData { get; set; }

        public FileContentResult? FileContentResult { get; set; }

        /// <summary>
        /// If no keeper is provided, leave things null.
        /// </summary>
        public KeeperServiceResponse()
        {

        }

        /// <summary>
        /// If a keeper was provided, set the keeper to the response Item.
        /// Example could be a lookup for a single keeper by Id.
        /// </summary>
        /// <param name="keeper"></param>
        public KeeperServiceResponse(IKeeperDto keeper)
        {
            Item = keeper;
        }

        /// <summary>
        /// If a list of keepers was provided, set the list to the response Item.
        /// Example could be a lookup for all keepers.
        /// </summary>
        /// <param name="keeper"></param>
        public KeeperServiceResponse(List<IKeeperDto> keeper)
        {
            List = keeper;
        }
    }

    public interface IGroupServiceResponse : IServiceResponse<IGroupDto>
    {
        IGroupComplexDto? ComplexDto { get; }
    }

    /// <summary>
    /// Group service response class.
    /// </summary>
    public class GroupServiceResponse : ServiceResponse<IGroupDto>, IGroupServiceResponse
    {
        public IGroupComplexDto? ComplexDto { get; set; }

        /// <summary>
        /// If no group is provided, leave things null.
        /// </summary>
        public GroupServiceResponse()
        {

        }

        /// <summary>
        /// If a group was provided, set the group to the response Item.
        /// Example could be a lookup for a single group by Id.
        /// </summary>
        /// <param name="group"></param>
        public GroupServiceResponse(IGroupDto group)
        {
            Item = group;
        }

        /// <summary>
        /// If a list of groups was provided, set the list to the response Item.
        /// Example could be a lookup for all groups.
        /// </summary>
        /// <param name="group"></param>
        public GroupServiceResponse(List<IGroupDto> group)
        {
            List = group;
        }
    }

    public interface IMembershipServiceResponse : IServiceResponse<IMembershipDto>
    {

    }

    /// <summary>
    /// Membership service response class.
    /// </summary>
    public class MembershipServiceResponse : ServiceResponse<IMembershipDto>, IMembershipServiceResponse
    {
        /// <summary>
        /// If no membership is provided, leave things null.
        /// </summary>
        public MembershipServiceResponse()
        {

        }

        /// <summary>
        /// If a membership was provided, set the membership to the response Item.
        /// Example could be a lookup for a single membership by Id.
        /// </summary>
        /// <param name="membership"></param>
        public MembershipServiceResponse(IMembershipDto membership)
        {
            Item = membership;
        }

        /// <summary>
        /// If a list of memberships was provided, set the list to the response Item.
        /// Example could be a lookup for all memberships.
        /// </summary>
        /// <param name="membership"></param>
        public MembershipServiceResponse(List<IMembershipDto> membership)
        {
            List = membership;
        }
    }

    public interface IPermitServiceResponse : IServiceResponse<IPermitDto>
    {

    }

    /// <summary>
    /// Permit service response class.
    /// </summary>
    public class PermitServiceResponse : ServiceResponse<IPermitDto>, IPermitServiceResponse
    {
        /// <summary>
        /// If no permit is provided, leave things null.
        /// </summary>
        public PermitServiceResponse()
        {

        }

        /// <summary>
        /// If a permit was provided, set the permit to the response Item.
        /// Example could be a lookup for a single permit by keeper id.
        /// </summary>
        /// <param name="permit"></param>
        public PermitServiceResponse(IPermitDto permit)
        {
            Item = permit;
        }

        /// <summary>
        /// If a list of permits was provided, set the list to the response Item.
        /// Example could be a lookup for all permits by keeper id.
        /// </summary>
        /// <param name="permit"></param>
        public PermitServiceResponse(List<IPermitDto> permit)
        {
            List = permit;
        }
    }
}
